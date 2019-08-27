using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an endpoint that handles a specific command or event.
    /// </summary>
    public interface IHandleAsyncMethodEndpoint : IMethodAttributeProvider
    {
        /// <summary>
        /// Returns the message handler this method has been implemented on.
        /// </summary>
        ITypeAttributeProvider MessageHandler
        {
            get;
        }

        /// <summary>
        /// Returns the parameter that represents the message to be handled.
        /// </summary>
        IParameterAttributeProvider MessageParameter
        {
            get;
        }

        /// <summary>
        /// Indicates whether this endpoint handles a command or an event.
        /// </summary>
        MessageKind MessageKind
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
        Task<IMessageHandlerOperationResult> InvokeAsync(object message, CancellationToken? token = null);
    }
}
