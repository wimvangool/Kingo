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
    public abstract class MicroServiceBusController : IMicroServiceBus, IHostedService, IDisposable
    {
        private readonly IMicroProcessor _processor;
        private readonly Lazy<StoreAndForwardQueue> _storeAndForwardQueue;
        private readonly Lazy<MicroServiceBusClient> _client;

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
            _storeAndForwardQueue = new Lazy<StoreAndForwardQueue>(CreateStoreAndForwardQueue, true);
            _client = new Lazy<MicroServiceBusClient>(CreateClient, true);
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
            $"{GetType().FriendlyName()} - [{StoreAndForwardQueue.GetType().FriendlyName()} --> {Client.GetType().FriendlyName()}]";

        #region [====== StoreAndForwardQueue ======]

        /// <summary>
        /// The <see cref="StoreAndForwardQueue"/> this controller uses to buffer any messages while
        /// a transaction is still in progress.
        /// </summary>
        protected StoreAndForwardQueue StoreAndForwardQueue =>
            _storeAndForwardQueue.Value;

        private StoreAndForwardQueue CreateStoreAndForwardQueue() =>
            CreateStoreAndForwardQueue(Client);

        /// <summary>
        /// Creates and returns the <see cref="StoreAndForwardQueue"/> that this controller will use to
        /// buffer any messages while a transaction is still in progress. By default
        /// </summary>
        /// <param name="bus">The bus to which the messages will be forwarded.</param>
        /// <returns></returns>
        protected virtual StoreAndForwardQueue CreateStoreAndForwardQueue(IMicroServiceBus bus) =>
            new ForwardOnlyQueue(bus);

        #endregion

        #region [====== Client ======]

        protected MicroServiceBusClient Client =>
            _client.Value;

        private MicroServiceBusClient CreateClient() =>
            CreateClient(_processor.CreateMicroServiceBusEndpoints());

        protected abstract MicroServiceBusClient CreateClient(IEnumerable<IMicroServiceBusEndpoint> endpoints);

        #endregion

        #region [====== StartAsync(...) & StopAsync(...) ======]

        /// <summary>
        /// Starts this controller by instructing the <see cref="StoreAndForwardQueue"/> and
        /// <see cref="Client"/> to start their message-senders and -receivers, based on the
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
        /// Stops this controller by instructing the <see cref="StoreAndForwardQueue"/> and
        /// <see cref="Client"/> to stop their message-senders and -receivers, based on the
        /// <see cref="MicroServiceBusModes" /> set for this controller.
        /// </summary>
        /// <param name="cancellationToken">
        /// Token that can be used to cancel the operation. If this token is signaled before
        /// the operation completes, the operation in which the controller is stopped in a graceful
        /// manner is aborted immediately and moved back to its stopped state.
        /// </param>
        public Task StopAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        #endregion

        #region [====== SendAsync ======]

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<IMessage> messages) =>
            throw new NotImplementedException();

        #endregion

        #region [====== Dispose ======]

        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
