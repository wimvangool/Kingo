using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents and endpoint that connects to a service bus to send specific messages.
    /// </summary>
    /// <typeparam name="TSerializedMessage">Type of the messages that are sent to the service bus.</typeparam>
    [MicroProcessorComponent(ServiceLifetime.Singleton)]
    public abstract class MicroServiceBus<TSerializedMessage> : MicroServiceBus, IHostedService
    {
        #region [====== HostedEndpointRelay ======]

        private sealed class HostedEndpointRelay : HostedEndpoint
        {
            private readonly MicroServiceBus<TSerializedMessage> _serviceBus;

            public HostedEndpointRelay(MicroServiceBus<TSerializedMessage> serviceBus)
            {
                _serviceBus = serviceBus;
            }

            public override string ToString() =>
                ToString(_serviceBus.GetType());

            protected override Task ConnectAsync(CancellationToken cancellationToken) =>
                _serviceBus.ConnectAsync(cancellationToken);

            protected override Task DisconnectAsync(CancellationToken cancellationToken) =>
                _serviceBus.DisconnectAsync(cancellationToken);
        }

        #endregion

        private readonly HostedEndpointRelay _hostedService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBus{T}" /> class.
        /// </summary>
        protected MicroServiceBus()
        {
            _hostedService = new HostedEndpointRelay(this);
        }

        /// <inheritdoc />
        public override string ToString() =>
            _hostedService.ToString();

        #region [====== IHostedService ======]

        /// <inheritdoc />
        public virtual Task StartAsync(CancellationToken cancellationToken) =>
            _hostedService.StartAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task StopAsync(CancellationToken cancellationToken) =>
            _hostedService.StopAsync(cancellationToken);

        /// <summary>
        /// Connects this endpoint to its resource. If <paramref name="cancellationToken"/> is signaled while
        /// the operation is still in progress, the operation is expected to be aborted and the endpoint
        /// must remain in its disconnected state.
        /// </summary>
        /// <param name="cancellationToken">Token used to signal cancellation of this operation.</param>        
        protected abstract Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Disconnects this endpoint from its resource. If <paramref name="cancellationToken"/> is signaled while
        /// the operation is still in progress, the operation is expected to be aborted immediately, and the state
        /// of the endpoint may be undetermined.
        /// </summary>
        /// <param name="cancellationToken">Token used to signal cancellation of this operation.</param>        
        protected abstract Task DisconnectAsync(CancellationToken cancellationToken);

        #endregion

        #region [====== IMicroServiceBus ======]        

        /// <inheritdoc />
        public sealed override Task PublishAsync(IEnumerable<IMessage> messages) =>
            PublishAsync(messages.WhereNotNull().Select(Serialize));

        /// <inheritdoc />
        public sealed override Task PublishAsync(IMessage message) =>
            PublishAsync(Serialize(message));

        /// <summary>
        /// Publishes all specified <paramref name="messages"/>.
        /// </summary>
        /// <param name="messages">The messages to publish.</param>        
        protected virtual async Task PublishAsync(IEnumerable<TSerializedMessage> messages)
        {            
            foreach (var message in messages)
            {
                await PublishAsync(message);
            }
        }

        /// <summary>
        /// Publishes the specified <paramref name="message" />.
        /// </summary>
        /// <param name="message">The message to publish.</param>        
        protected abstract Task PublishAsync(TSerializedMessage message);

        /// <summary>
        /// Serializes the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <returns>The serialized version of the specified <paramref name="message"/>.</returns>       
        protected abstract TSerializedMessage Serialize(IMessage message);

        #endregion
    }
}
