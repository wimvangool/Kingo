using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an endpoint that can handle a specific set of messages.
    /// </summary>
    public interface IMessageHandlerEndpoint
    {
        /// <summary>
        /// The type of the message handler that is used by this endpoint.
        /// </summary>
        Type MessageHandlerType
        {
            get;
        }

        /// <summary>
        /// The set of message types that are supported by this endpoint.
        /// </summary>
        IReadOnlyCollection<Type> MessageTypes
        {
            get;
        }

        /// <summary>
        /// Handles the specified <paramref name="message" /> and returns the resulting event stream. If the specified
        /// <paramref name="message"/> is not supported by this endpoint, an empty result is returned.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="token">A token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// </returns>
        Task<HandleAsyncResult> HandleAsync(object message, CancellationToken token);
    }
}
