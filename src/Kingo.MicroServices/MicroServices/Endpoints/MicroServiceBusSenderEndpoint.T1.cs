using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents and endpoint that connects to a service bus to send specific messages.
    /// </summary>
    /// <typeparam name="TMessageEnvelope">Type of the messages that are sent to the service bus.</typeparam>
    public abstract class MicroServiceBusSenderEndpoint<TMessageEnvelope> : MicroServiceBusEndpoint<TMessageEnvelope>, IMicroServiceBus
    {
        /// <inheritdoc />
        public Task PublishAsync(IEnumerable<object> messages) =>
            PublishAsync(messages.Select(message => Serializer.Serialize(message)));

        /// <inheritdoc />
        public Task PublishAsync(object message) =>
            PublishAsync(Serializer.Serialize(message));

        /// <summary>
        /// Publishes all specified <paramref name="messages"/>.
        /// </summary>
        /// <param name="messages">The messages to publish.</param>        
        protected virtual async Task PublishAsync(IEnumerable<TMessageEnvelope> messages)
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
        protected abstract Task PublishAsync(TMessageEnvelope message);
    }
}
