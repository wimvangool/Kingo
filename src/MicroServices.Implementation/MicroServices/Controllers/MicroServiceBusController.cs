using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;
using Kingo.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a controller that can send and receive messages to and from a
    /// service-bus and routes any received message to a <see cref="IMicroServiceBusEndpoint" />
    /// for further processing.
    /// </summary>
    [MicroProcessorComponent(ServiceLifetime.Singleton)]
    public abstract class MicroServiceBusController : AsyncDisposable, IMicroServiceBus, IHostedService
    {
        #region [====== State ======]

        private abstract class State : Disposable, IMicroServiceBus
        {
            public override string ToString() =>
                GetType().FriendlyName().RemovePostfix(nameof(State));

            public abstract Task StartAsync(CancellationToken token);

            public abstract Task StopAsync();

            public abstract Task SendAsync(IEnumerable<IMessage> messages);
        }

        #endregion

        #region [====== StoppedState ======]

        private sealed class StoppedState : State
        {
            private readonly MicroServiceBusController _controller;

            public StoppedState(MicroServiceBusController controller)
            {
                _controller = controller;
            }

            public override Task StartAsync(CancellationToken token) =>
                StartAsync(new StartingState(_controller, token));

            private async Task StartAsync(StartingState startingState)
            {
                // When starting the controller, we first move to the starting state. While in this state,
                // the controller's Outbox and ServiceBus are started. If this process, for whatever reason,
                // fails, then the controller is reverted back to a stopped state.
                if (_controller.MoveToState(this, startingState))
                {
                    try
                    {
                        // While the controller is being started, another thread may have called StopAsync() on the controller.
                        // If that's the case, the specified token will be signaled, so that the startup-operation knows it
                        // can cancel its operation.
                        // In addition, we can no longer move into the started-state (since the current state is no longer the
                        // starting state). This means this state can then immediately be disposed.
                        await _controller.StartControllerAsync(startingState.Token).ConfigureAwait(false);

                        var startedState = new StartedState(_controller);

                        if (_controller.MoveToState(startingState, startedState))
                        {
                            return;
                        }
                        startedState.Dispose();
                    }
                    catch
                    {
                        _controller.MoveToState(startingState, new StoppedState(_controller));
                        throw;
                    }
                }
                else
                {
                    throw NewControllerAlreadyStartedException(_controller);
                }
            }

            public override Task StopAsync() =>
                Task.CompletedTask;

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _controller.Outbox.SendAsync(messages);
        }

        #endregion

        #region [====== StartingState ======]

        private sealed class StartingState : State
        {
            private readonly MicroServiceBusController _controller;
            private readonly CancellationTokenSource _tokenSource;

            public StartingState(MicroServiceBusController controller, CancellationToken token)
            {
                _controller = controller;
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            }

            public CancellationToken Token =>
                _tokenSource.Token;

            public override Task StartAsync(CancellationToken token) =>
                throw NewControllerAlreadyStartedException(_controller);

            public override Task StopAsync()
            {
                // If StopAsync is called when the controller is still starting, we signal to
                // the startup-operation that it should cancel its efforts.
                if (_controller.MoveToState(this, new StoppedState(_controller)))
                {
                    _tokenSource.Cancel();
                }
                return Task.CompletedTask;
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _controller.Outbox.SendAsync(messages);

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _tokenSource.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        #endregion

        #region [====== StartedState ======]

        private sealed class StartedState : State
        {
            private readonly MicroServiceBusController _controller;

            public StartedState(MicroServiceBusController controller)
            {
                _controller = controller;
            }

            public override Task StartAsync(CancellationToken token) =>
                throw NewControllerAlreadyStartedException(_controller);

            public override Task StopAsync() =>
                StopAsync(new StoppingState(_controller));

            private async Task StopAsync(StoppingState stoppingState)
            {
                if (_controller.MoveToState(this, stoppingState))
                {
                    try
                    {
                        await _controller.StopControllerAsync().ConfigureAwait(false);
                    }
                    finally
                    {
                        _controller.MoveToState(stoppingState, new StoppedState(_controller));
                    }
                }
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _controller.Outbox.SendAsync(messages);
        }

        #endregion

        #region [====== StoppingState ======]

        private sealed class StoppingState : State
        {
            private readonly MicroServiceBusController _controller;

            public StoppingState(MicroServiceBusController controller)
            {
                _controller = controller;
            }

            public override Task StartAsync(CancellationToken token) =>
                throw NewControllerAlreadyStartedException(_controller);

            public override Task StopAsync() =>
                Task.CompletedTask;

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _controller.Outbox.SendAsync(messages);
        }

        #endregion

        #region [====== DisposedState ======]

        private sealed class DisposedState : State
        {
            private readonly MicroServiceBusController _controller;

            public DisposedState(MicroServiceBusController controller)
            {
                _controller = controller;
            }

            public override Task StartAsync(CancellationToken token) =>
                throw NewControllerDisposedException(_controller);

            public override Task StopAsync() =>
                throw NewControllerDisposedException(_controller);

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                throw NewControllerDisposedException(_controller);
        }

        #endregion

        private readonly IMicroProcessor _processor;
        private readonly Lazy<MicroServiceBusOutbox> _outbox;
        private readonly Lazy<MicroServiceBusInbox> _inbox;
        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusController" /> class.
        /// </summary>
        /// <param name="processor">The processor that will be processing and producing all commands and events.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusController(IMicroProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _outbox = new Lazy<MicroServiceBusOutbox>(CreateOutbox, true);
            _inbox = new Lazy<MicroServiceBusInbox>(CreateInbox, true);
            _state = new StoppedState(this);
        }

        /// <summary>
        /// The processor that will be processing all commands and events.
        /// </summary>
        protected IMicroProcessor Processor =>
            _processor;

        /// <summary>
        /// Gets the options that were set for this controller.
        /// </summary>
        protected abstract MicroServiceBusControllerOptions Options
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({_state})";

        #region [====== Outbox ======]

        /// <summary>
        /// The <see cref="Outbox"/> this controller uses to buffer any messages while
        /// a transaction is still in progress.
        /// </summary>
        protected MicroServiceBusOutbox Outbox =>
            _outbox.Value;

        private MicroServiceBusOutbox CreateOutbox() =>
            CreateOutbox(Inbox);

        /// <summary>
        /// Creates and returns the <see cref="Outbox"/> that will be used by this controller to
        /// temporarily store any messages produced by the processor, which the outbox will
        /// then forward to the specified <paramref name="serviceBus"/> at the appropriate time.
        /// </summary>
        /// <param name="serviceBus">The bus to which the messages will be forwarded.</param>
        /// <returns>A new <see cref="MicroServiceBusOutbox"/>.</returns>
        protected virtual MicroServiceBusOutbox CreateOutbox(IMicroServiceBus serviceBus) =>
            new DirectSendOutbox(serviceBus);

        #endregion

        #region [====== Inbox ======]

        protected MicroServiceBusInbox Inbox =>
            _inbox.Value;

        private MicroServiceBusInbox CreateInbox() =>
            CreateInbox(_processor.CreateMicroServiceBusEndpoints());

        /// <summary>
        /// Creates and returns a new <see cref="MicroServiceBusInbox"/> that will be used by this controller to
        /// send and/or receive messages from a service-bus, depending on the configuration of this controller.
        /// </summary>
        /// <param name="endpoints">
        /// The endpoints exposed by the processor that are configured to receive messages from the service-bus.
        /// </param>
        /// <returns>A new <see cref="MicroServiceBusInbox"/>.</returns>
        protected abstract MicroServiceBusInbox CreateInbox(IEnumerable<IMicroServiceBusEndpoint> endpoints);

        #endregion

        #region [====== StartAsync(...), StopAsync(...) & DisposeAsync(...) ======]

        /// <summary>
        /// Starts this controller by instructing the <see cref="Outbox"/> and
        /// <see cref="Inbox"/> to start their message-senders and -receivers, based on the
        /// <see cref="MicroServiceBusModes" /> set for this controller.
        /// </summary>
        /// <param name="cancellationToken">
        /// Token that can be used to cancel the operation. If this token is signaled before
        /// the operation completes, the controller will move back to its stopped state.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// This controller is either already starting or has already started.
        /// </exception>
        public Task StartAsync(CancellationToken cancellationToken) =>
            _state.StartAsync(cancellationToken);

        private async Task StartControllerAsync(CancellationToken cancellationToken)
        {
            if (Options.Modes.HasFlag(MicroServiceBusModes.Send))
            {
                // When starting Send-mode, we must first start the Sender of the ServiceBus. This will allow
                // all messages forwarded by the Outbox to be sent as they are handed over. As soon as the ServiceBus
                // is ready, we instruct the Outbox to start forwarding messages and then to also accept new messages.
                await Inbox.StartSendingMessagesAsync(cancellationToken);
                await Outbox.StartReceivingMessagesAsync(cancellationToken);
                await Outbox.StartSendingMessagesAsync(cancellationToken);
            }
            if (Options.Modes.HasFlag(MicroServiceBusModes.Receive))
            {
                await Inbox.StartReceivingMessagesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Stops this controller by instructing the <see cref="Outbox"/> and
        /// <see cref="Inbox"/> to stop their message-senders and -receivers, based on the
        /// <see cref="MicroServiceBusModes" /> set for this controller.
        /// </summary>
        /// <param name="cancellationToken">
        /// Token that can be used to cancel the operation. If this token is signaled before
        /// the operation completes, the operation in which the controller is stopped in a graceful
        /// manner is aborted immediately and moved back to its stopped state.
        /// </param>
        public Task StopAsync(CancellationToken cancellationToken) =>
            _state.StopAsync().OrAbort(cancellationToken);

        private Task StopControllerAsync() =>
            Task.WhenAll(StopSendingAndReceivingMessages());

        private IEnumerable<Task> StopSendingAndReceivingMessages()
        {
            yield return Inbox.StopReceivingMessagesAsync();
            yield return Outbox.StopSendingMessagesAsync();
            yield return Outbox.StopReceivingMessagesAsync();
            yield return Inbox.StopSendingMessagesAsync();
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsync(DisposeContext context)
        {
            if (context != DisposeContext.Finalizer)
            {
                Interlocked.Exchange(ref _state, new DisposedState(this)).Dispose();

                if (_outbox.IsValueCreated)
                {
                    await _outbox.Value.DisposeAsync();
                }
                if (_inbox.IsValueCreated)
                {
                    await _inbox.Value.DisposeAsync();
                }
            }
            await base.DisposeAsync(context);
        }

        private bool MoveToState(State oldState, State newState)
        {
            try
            {
                return Interlocked.CompareExchange(ref _state, newState, oldState) == oldState;
            }
            finally
            {
                oldState.Dispose();
            }
        }

        private static Exception NewControllerAlreadyStartedException(object controller)
        {
            var messageFormat = ExceptionMessages.MicroServiceBusController_ControllerAlreadyStarted;
            var message = string.Format(messageFormat, controller.GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        private static Exception NewControllerDisposedException(MicroServiceBusController controller) =>
            controller.NewObjectDisposedException();

        #endregion

        #region [====== SendAsync ======]

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<IMessage> messages) =>
            _state.SendAsync(messages);

        #endregion
    }
}
