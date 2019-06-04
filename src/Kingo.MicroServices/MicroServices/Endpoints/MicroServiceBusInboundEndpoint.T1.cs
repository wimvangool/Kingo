using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents and endpoint that connects to a service bus to receive and process specific
    /// messages.
    /// </summary>
    /// <typeparam name="TMessageEnvelope">Type of the messages that are received from the service bus.</typeparam>
    public abstract class MicroServiceBusInboundEndpoint<TMessageEnvelope> : MicroServiceBusEndpoint<TMessageEnvelope>
    {       
        /// <summary>
        /// The message handler that will handle all incoming messages.
        /// </summary>
        protected abstract IMessageHandlerEndpoint MessageHandler
        {
            get;
        }

        /// <summary>
        /// The outbound endpoint that will publish all events that are produced while handling the incoming
        /// messages.
        /// </summary>
        protected abstract IMicroServiceBus ServiceBus
        {
            get;
        }

        /// <summary>
        /// Handles the received <paramref name="message" /> by deserializing it, processing it
        /// with the associated <see cref="MessageHandler" /> and publishing the resulting events.
        /// </summary>
        /// <param name="message">The message that was received.</param>
        /// <param name="token">Token that may signal the cancellation of the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Task HandleAsync(TMessageEnvelope message, CancellationToken token) =>
            HandleAsync(Serializer.Deserialize(message), token);

        /// <summary>
        /// Handles the received <paramref name="message" /> by processing it
        /// with the associated <see cref="MessageHandler" /> and publishing the resulting events.        
        /// </summary>
        /// <param name="message">The message that was received.</param>
        /// <param name="token">Token that may signal the cancellation of the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected virtual async Task HandleAsync(object message, CancellationToken token) =>
            await HandleResultAsync(await MessageHandler.HandleAsync(message, token));

        /// <summary>
        /// Handles the result that was returned by the <see cref="MessageHandler" /> when handling a
        /// specific message by publishing all events.
        /// </summary>
        /// <param name="result">The result to handle.</param>        
        protected virtual Task HandleResultAsync(HandleAsyncResult result) =>
            ServiceBus.PublishAsync(result.Events);
    }
}
