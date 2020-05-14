using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an endpoint that can process messages that are received from a <see cref="IMicroServiceBus" />.
    /// </summary>
    public interface IMicroServiceBusEndpoint
    {
        /// <summary>
        /// Indicates what kind of message is processed by this endpoint.
        /// </summary>
        MessageKind MessageKind
        {
            get;
        }

        /// <summary>
        /// Returns the name of this endpoint.
        /// </summary>
        string Name
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
        /// Returns the <see cref="ParameterInfo" /> that represents the context of the operation.
        /// </summary>
        ParameterInfo ContextParameterInfo
        {
            get;
        }

        /// <summary>
        /// Processes the specified <paramref name="message"/> and returns the result.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// If the specified <paramref name="message"/>  is not supported by this endpoint, it is ignored and an empty
        /// result will be returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="message"/> is not an instance of a type that is supported by this endpoint.
        /// </exception>
        Task<MessageHandlerOperationResult<object>> ProcessAsync(object message) =>
            ProcessAsync(message, MessageHeader.Unspecified);

        /// <summary>
        /// Processes the specified <paramref name="message"/> and returns the result.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <param name="messageHeader">Header of the message.</param>
        /// <param name="token">Optional token that can be used to cancel the operation.</param>
        /// <returns>
        /// The result of the operation, which includes all published events and the number of message handlers that were invoked.
        /// If the specified <paramref name="message"/>  is not supported by this endpoint, it is ignored and an empty
        /// result will be returned.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="message"/> is not an instance of a type that is supported by this endpoint.
        /// </exception>
        Task<MessageHandlerOperationResult<object>> ProcessAsync(object message, MessageHeader messageHeader, CancellationToken? token = null);
    }
}
