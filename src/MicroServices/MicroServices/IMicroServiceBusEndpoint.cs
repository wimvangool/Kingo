using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an endpoint that handles a specific command or event.
    /// </summary>
    public interface IMicroServiceBusEndpoint
    {
        /// <summary>
        /// Returns the name of the service this endpoint is part of.
        /// </summary>
        string ServiceName
        {
            get;
        }

        /// <summary>
        /// Returns the type of the message handler this endpoint is implemented on.
        /// </summary>
        Type MessageHandlerType
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="System.Reflection.MethodInfo"/> of the method.
        /// </summary>
        MethodInfo MethodInfo
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="ParameterInfo" /> that represents the message to be handled.
        /// </summary>
        ParameterInfo MessageParameterInfo
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="ParameterInfo" /> that represents the context that is supplied to the method.
        /// </summary>
        ParameterInfo ContextParameterInfo
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
        Task<IMessageHandlerOperationResult> InvokeAsync(IMessage message, CancellationToken? token = null);
    }
}
