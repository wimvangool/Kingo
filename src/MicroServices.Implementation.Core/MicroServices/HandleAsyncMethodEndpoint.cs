using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represent an endpoint to a specific <see cref="IMessageHandler{TMessage}.HandleAsync"/> method
    /// of a message handler that is exposed as an endpoint.
    /// </summary>
    public abstract class HandleAsyncMethodEndpoint : HandleAsyncMethod
    {
        internal HandleAsyncMethodEndpoint(HandleAsyncMethod method) :
            base(method) { }

        /// <summary>
        /// Returns the <see cref="MessageKind"/> of the message that is handled by this endpoint.
        /// </summary>
        public abstract MessageKind MessageKind
        {
            get;
        }

        /// <summary>
        /// Invokes the method of the associated message handler with the specified <paramref name="message" />
        /// and returns its result.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// If the specified <paramref name="message"/>  is not supported by this endpoint, it is ignored and an empty
        /// result will be returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        public abstract Task<MessageHandlerOperationResult> InvokeAsync(object message, CancellationToken? token = null);

        /// <inheritdoc />
        public override string ToString() =>
            $"{MessageHandler.Type.FriendlyName()}.{Info.Name}([{MessageKind}] {MessageParameter.Type.FriendlyName()}, ...)";
    }
}
