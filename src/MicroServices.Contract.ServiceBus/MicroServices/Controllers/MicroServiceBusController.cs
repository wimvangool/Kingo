using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class MicroServiceBusController : Disposable, IMicroServiceBus, IHostedService
    {
        #region [====== State ======]

        private abstract class State : Disposable, IMicroServiceBus, IHostedService
        {
            protected State(MicroServiceBusController controller)
            {
                Controller = controller;
            }

            protected MicroServiceBusController Controller
            {
                get;
            }

            public override string ToString() =>
                GetType().FriendlyName().RemovePostfix(nameof(State));

            public abstract Task StartAsync(CancellationToken cancellationToken);

            public abstract Task StopAsync(CancellationToken cancellationToken);

            public abstract Task RouteAsync(IEnumerable<IMessageToDispatch> commands);

            public abstract Task SendAsync(IEnumerable<IMessageToDispatch> commands);

            public abstract Task PublishAsync(IEnumerable<IMessageToDispatch> events);

            public override void Dispose()
            {
                // When an attempt is made to dispose the controller, we switch to
                // the disposed state. If this succeeds, we let the controller dispose
                // its resources. The only reason why this would not succeed is when two
                // or more threads attempt to dispose of the controller simultaneously,
                // in which case only one will succeed.
                var newState = new DisposedState(Controller);

                if (TryMoveToState(newState))
                {
                    Controller.Dispose(true);
                }
            }

            protected bool TryMoveToState(State newState)
            {
                if (Interlocked.CompareExchange(ref Controller._state, newState, this) == this)
                {
                    // When the state-change was successful, the old state is disposed, so that it can clean up its resources.
                    Dispose(true);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region [====== StoppedState ======]

        private sealed class StoppedState : State
        {
            public StoppedState(MicroServiceBusController controller) :
                base(controller) { }

            public override Task StartAsync(CancellationToken cancellationToken)
            {
                // When attempting to start the controller from the stopped state, we
                // change the state to the started state. Only one thread is allowed
                // to perform this state-change; other threads will be redirected to
                // the new state, possibly resulting in an exception.
                var startedState = new StartedState(Controller);

                if (TryMoveToState(startedState))
                {
                    return startedState.StartAsync(cancellationToken, true);
                }
                return Controller._state.StartAsync(cancellationToken);
            }

            public override Task StopAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;

            public override Task RouteAsync(IEnumerable<IMessageToDispatch> commands) =>
                throw Controller.NewCannotSendCommandsException();

            public override Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
                throw Controller.NewCannotSendCommandsException();

            public override Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
                throw Controller.NewCannotPublishEventsException();
        }

        #endregion

        #region [====== StartedState ======]

        private sealed class StartedState : State
        {
            private readonly IMicroServiceBusClient _client;

            public StartedState(MicroServiceBusController controller) :
                this(controller, new DisconnectedClient(controller)) { }

            private StartedState(MicroServiceBusController controller, IMicroServiceBusClient client) :
                base(controller)
            {
                _client = client;
            }

            public override Task StartAsync(CancellationToken cancellationToken) =>
                StartAsync(cancellationToken, false);

            public async Task StartAsync(CancellationToken cancellationToken, bool isStarting)
            {
                if (isStarting)
                {
                    // When starting, we connect all endpoints to the service-bus. We then
                    // attempt to change the controller's state to a started-state where we have a
                    // fully initialized client. 
                    var client = await Controller.ConnectToServiceBusAsync(cancellationToken).ConfigureAwait(false);
                    if (client == null)
                    {
                        // If client is null, then the start-operation was aborted while connecting.
                        // In that case, we attempt to move back to the stopped state.
                        TryMoveToState(new StoppedState(Controller));
                        return;
                    }
                    var startedState = new StartedState(Controller, client);

                    if (TryMoveToState(startedState))
                    {
                        return;
                    }
                    // If the controller's state has been changed while
                    // the client was starting, we immediately dispose of the created state.
                    startedState.Dispose(true);
                    return;
                }
                throw Controller.NewAlreadyStartedException();
            }

            public override Task StopAsync(CancellationToken cancellationToken)
            {
                var stoppedState = new StoppedState(Controller);

                if (TryMoveToState(stoppedState))
                {
                    return stoppedState.StopAsync(cancellationToken);
                }
                return Controller._state.StopAsync(cancellationToken);
            }

            public override Task RouteAsync(IEnumerable<IMessageToDispatch> commands)
            {
                // Each command is paired with the controller that is capable of sending that command
                // (i.e. the service contract of that controller contains that command). Then, every
                // command is sent by that/those controller(s) in parallel fashion. For example:
                // - ControllerX: [CommandA, CommandB]
                // - ControllerY: [CommandA, CommandC]
                // - ControllerZ: []
                var sendOperations =
                    from commandsPerController in MatchCommandsWithControllers(commands.ToArray())
                    let controller = commandsPerController.Key
                    let commandsToSend = commandsPerController.Value
                    where commandsToSend.Length > 0
                    select controller._state.SendAsync(commandsToSend);

                return Task.WhenAll(sendOperations);
            }

            private IDictionary<MicroServiceBusController, IMessageToDispatch[]> MatchCommandsWithControllers(IReadOnlyCollection<IMessageToDispatch> commands)
            {
                var commandsPerController = new Dictionary<MicroServiceBusController, IMessageToDispatch[]>();
                if (commands.Count > 0)
                {
                    foreach (var controller in ResolveControllers())
                    {
                        commandsPerController.Add(controller, controller.FilterMessagesOfServiceContract(commands).ToArray());
                    }
                }
                return commandsPerController;
            }

            private IEnumerable<MicroServiceBusController> ResolveControllers() =>
                Controller._processor.ServiceProvider.GetServices<MicroServiceBusController>();

            public override Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
                _client.SendAsync(commands);

            public override Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
                _client.PublishAsync(events);

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
                {
                    return;
                }
                if (disposing)
                {
                    _client.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        #endregion

        #region [====== DisposedState ======]

        private sealed class DisposedState : State
        {
            public DisposedState(MicroServiceBusController controller) :
                base(controller) { }

            public override Task StartAsync(CancellationToken cancellationToken) =>
                throw Controller.NewObjectDisposedException();

            public override Task StopAsync(CancellationToken cancellationToken) =>
                throw Controller.NewObjectDisposedException();

            public override Task RouteAsync(IEnumerable<IMessageToDispatch> commands) =>
                throw Controller.NewObjectDisposedException();

            public override Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
                throw Controller.NewObjectDisposedException();

            public override Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
                throw Controller.NewObjectDisposedException();

            // When dispose is called while we're already in the DisposedState, we don't have to do anything...
            public override void Dispose() { }
        }

        #endregion

        #region [====== DisconnectedClient ======]

        private sealed class DisconnectedClient : MicroServiceBusConnection, IMicroServiceBusClient
        {
            private readonly MicroServiceBusController _controller;

            public DisconnectedClient(MicroServiceBusController controller)
            {
                _controller = controller;
            }

            public Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
                throw _controller.NewCannotSendCommandsException();

            public Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
                throw _controller.NewCannotPublishEventsException();

            public Task ConnectToEndpointAsync(IMicroServiceBusEndpoint endpoint) =>
                Task.CompletedTask;
        }

        #endregion

        private readonly IMicroServiceBusProcessor _processor;
        private readonly Lazy<ServiceContract> _serviceContract;
        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusController" /> class.
        /// </summary>
        /// <param name="processor">The processor that will be processing all commands and/or events.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusController(IMicroServiceBusProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _serviceContract = new Lazy<ServiceContract>(DefineServiceContract, true);
            _state = new StoppedState(this);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({_state})";

        #region [====== IHostedService ======]

        /// <summary>
        /// The start method creates a new <see cref="IMicroServiceBusClient"/> and attempts to connect
        /// every <see cref="IMicroServiceBusEndpoint"/> that is provided by the <see cref="IMicroServiceBusProcessor"/>
        /// to the service-bus through this client. If <paramref name="cancellationToken"/> is signaled before the
        /// client is fully created and connected, the operation is aborted and the controller remains in the
        /// stopped/disconnected state.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to abort the operation.</param>
        /// <exception cref="ObjectDisposedException">
        /// The controller has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The controller has already been started.
        /// </exception>
        public virtual Task StartAsync(CancellationToken cancellationToken) =>
            _state.StartAsync(cancellationToken);

        /// <summary>
        /// The stop method closes and disposes the <see cref="IMicroServiceBusClient"/> gracefully,
        /// unless the specified <paramref name="cancellationToken"/> is signaled to abort the operation
        /// quickly.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to abort the operation.</param>
        /// <exception cref="ObjectDisposedException">
        /// The controller has already been disposed.
        /// </exception>
        public virtual Task StopAsync(CancellationToken cancellationToken) =>
            _state.StopAsync(cancellationToken);

        #endregion

        #region [====== IMicroServiceBus ======]

        /// <inheritdoc />
        public virtual Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
            _state.RouteAsync(commands);

        /// <inheritdoc />
        public virtual Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
            _state.PublishAsync(FilterMessagesOfServiceContract(events));

        #endregion

        #region [====== ConnectToServiceBus ======]

        private async Task<IMicroServiceBusClient> ConnectToServiceBusAsync(CancellationToken token)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            var client = await CreateClientAsync().ConfigureAwait(false);

            if (IsCancellationRequested(token, ref client))
            {
                return client;
            }
            foreach (var endpoint in _processor.CreateMicroServiceBusEndpoints())
            {
                await client.ConnectToEndpointAsync(endpoint).OrAbort(token).ConfigureAwait(false);

                if (IsCancellationRequested(token, ref client))
                {
                    return client;
                }
            }
            return client;
        }

        private static bool IsCancellationRequested(CancellationToken token, ref IMicroServiceBusClient client)
        {
            // When cancellation is requested while the service-bus client is being created and connected,
            // we immediately dispose of the created client and return null.
            if (token.IsCancellationRequested)
            {
                Interlocked.Exchange(ref client, null).Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates and returns a new <see cref="IMicroServiceBusClient"/> that can be used send and receive
        /// messages to and from a service-bus.
        /// </summary>
        /// <returns>A new client.</returns>
        protected abstract Task<IMicroServiceBusClient> CreateClientAsync();

        #endregion

        #region [====== Dispose ======]

        /// <inheritdoc />
        public override void Dispose() =>
            _state.Dispose();

        #endregion

        #region [====== ServiceContract ======]

        private ServiceContract ServiceContract =>
            _serviceContract.Value;

        private ServiceContract DefineServiceContract() =>
            ServiceContract.FromTypeSet(DefineServiceContract(TypeSet.Empty));

        /// <summary>
        /// Creates and returns a <see cref="TypeSet"/> that defines which types are part of this service's service contract.
        /// </summary>
        /// <param name="serviceContract">The default service contract.</param>
        /// <returns>A <see cref="TypeSet"/> that contains all messages that are part of this service's service contract.</returns>
        protected abstract TypeSet DefineServiceContract(TypeSet serviceContract);

        private IEnumerable<IMessageToDispatch> FilterMessagesOfServiceContract(IEnumerable<IMessageToDispatch> messages) =>
            messages.Where(command => ServiceContract.Contains(command.Content.GetType()));

        #endregion

        #region [====== Exception Factory Methods ======]

        private Exception NewCannotSendCommandsException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBusController_CannotPublishEvents;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        private Exception NewCannotPublishEventsException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBusController_CannotPublishEvents;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        private Exception NewAlreadyStartedException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBusController_AlreadyStarted;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
