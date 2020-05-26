using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;
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
        private readonly IMicroProcessor _processor;
        private readonly Lazy<MicroServiceBusOutbox> _outbox;
        private readonly Lazy<MicroServiceBus> _serviceBus;

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
            _serviceBus = new Lazy<MicroServiceBus>(CreateServiceBus, true);
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
            $"{GetType().FriendlyName()} - [{Outbox.GetType().FriendlyName()} --> {ServiceBus.GetType().FriendlyName()}]";

        #region [====== Outbox ======]

        /// <summary>
        /// The <see cref="Outbox"/> this controller uses to buffer any messages while
        /// a transaction is still in progress.
        /// </summary>
        protected MicroServiceBusOutbox Outbox =>
            _outbox.Value;

        private MicroServiceBusOutbox CreateOutbox() =>
            CreateOutbox(ServiceBus);

        /// <summary>
        /// Creates and returns the <see cref="Outbox"/> that will be used by this controller to
        /// temporarily store any messages produced by the processor, which the outbox will
        /// then forward to the specified <paramref name="bus"/> at the appropriate time.
        /// </summary>
        /// <param name="bus">The bus to which the messages will be forwarded.</param>
        /// <returns>A new <see cref="MicroServiceBusOutbox"/>.</returns>
        protected virtual MicroServiceBusOutbox CreateOutbox(IMicroServiceBus bus) =>
            new DirectSendOutbox(bus);

        #endregion

        #region [====== ServiceBus ======]

        protected MicroServiceBus ServiceBus =>
            _serviceBus.Value;

        private MicroServiceBus CreateServiceBus() =>
            CreateServiceBus(_processor.CreateMicroServiceBusEndpoints());

        /// <summary>
        /// Creates and returns a new <see cref="MicroServiceBus"/> that will be used by this controller to
        /// send and/or receive messages from a service-bus, depending on the configuration of this controller.
        /// </summary>
        /// <param name="endpoints">
        /// The endpoints exposed by the processor that are configured to receive messages from the service-bus.
        /// </param>
        /// <returns>A new <see cref="MicroServiceBus"/>.</returns>
        protected abstract MicroServiceBus CreateServiceBus(IEnumerable<IMicroServiceBusEndpoint> endpoints);

        #endregion

        #region [====== StartAsync(...), StopAsync(...) & DisposeAsync(...) ======]

        /// <summary>
        /// Starts this controller by instructing the <see cref="Outbox"/> and
        /// <see cref="ServiceBus"/> to start their message-senders and -receivers, based on the
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
            throw new NotImplementedException();

        /// <summary>
        /// Stops this controller by instructing the <see cref="Outbox"/> and
        /// <see cref="ServiceBus"/> to stop their message-senders and -receivers, based on the
        /// <see cref="MicroServiceBusModes" /> set for this controller.
        /// </summary>
        /// <param name="cancellationToken">
        /// Token that can be used to cancel the operation. If this token is signaled before
        /// the operation completes, the operation in which the controller is stopped in a graceful
        /// manner is aborted immediately and moved back to its stopped state.
        /// </param>
        public Task StopAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            if (_outbox.IsValueCreated)
            {
                await _outbox.Value.DisposeAsync();
            }
            if (_serviceBus.IsValueCreated)
            {
                await _serviceBus.Value.DisposeAsync();
            }
            await base.DisposeAsync();
        }

        #endregion

        #region [====== SendAsync ======]

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<IMessage> messages) =>
            Outbox.SendAsync(messages);

        #endregion
    }
}
