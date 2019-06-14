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
    public abstract class MicroServiceBusReceiverEndpoint<TMessageEnvelope> : MicroServiceBusEndpoint<TMessageEnvelope>
    {       
        /// <summary>
        /// The message handler that will handle all incoming messages.
        /// </summary>
        protected abstract HandleAsyncMethodEndpoint MethodEndpoint
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
        /// with the associated <see cref="MethodEndpoint" /> and publishing the resulting events.
        /// </summary>
        /// <param name="message">The message that was received.</param>
        /// <param name="token">Token that may signal the cancellation of the operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        protected async Task HandleAsync(TMessageEnvelope message, CancellationToken token) =>
            await HandleResultAsync(await HandleAsync(Serializer.Deserialize(message), token));

        /// <summary>
        /// Handles the received <paramref name="message" /> and returns the result.       
        /// </summary>
        /// <param name="message">The message that was received.</param>
        /// <param name="token">Token that may signal the cancellation of the operation.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        protected virtual async Task<MessageHandlerOperationResult> HandleAsync(object message, CancellationToken token) =>
            await MethodEndpoint.InvokeAsync(message, token);

        /// <summary>
        /// Handles the result that was returned by the <see cref="MethodEndpoint" /> when handling a
        /// specific message by publishing all events.
        /// </summary>
        /// <param name="result">The result to handle.</param>        
        protected virtual Task HandleResultAsync(MessageHandlerOperationResult result) =>
            ServiceBus.PublishAsync(result.Events);
    }
}
