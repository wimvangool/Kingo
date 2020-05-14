using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an exception that can be thrown while a <see cref="MicroProcessorOperation" /> is in progress.
    /// The <see cref="InternalOperationException"/> signals to the <see cref="IMicroProcessor"/> that the operation can be aborted,
    /// and provides the opportunity to the processor to return/throw the appropriate <see cref="MicroProcessorOperationException"/>,
    /// depending on the particular operation and the when the exception was thrown.
    /// </summary>
    [Serializable]
    public abstract class InternalOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalOperationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        protected InternalOperationException(string message = null, Exception innerException = null) :
            base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalOperationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InternalOperationException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        internal MicroProcessorOperationException ToMicroProcessorOperationException(MicroProcessorOperationStackTrace operationStackTrace)
        {
            if (IsBadRequest(operationStackTrace))
            {
                return ToBadRequestException(operationStackTrace);
            }
            return ToInternalServerErrorException(operationStackTrace);
        }

        /// <summary>
        /// Determines if this exception was caused by a bad request, and if so, should consequently be
        /// converted to a <see cref="BadRequestException" /> by the processor. By default, this method
        /// returns <c>true</c> if all messages in the stack-trace are of kind <see cref="MessageKind.Command"/>
        /// or <see cref="MessageKind.Request"/>.
        /// </summary>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>
        /// <returns>
        /// <c>true</c> if this exception should be converted to a <see cref="BadRequestException"/>; otherwise <c>false</c>.
        /// </returns>
        protected abstract bool IsBadRequest(MicroProcessorOperationStackTrace operationStackTrace);

        protected static bool IsRequestOperation(MicroProcessorOperationStackItem operation) =>
            IsRequestMessage(operation.Message);

        private static bool IsRequestMessage(IMessage message) =>
            message.Kind == MessageKind.Command || message.Kind == MessageKind.Request;

        /// <summary>
        /// Creates and returns this exception as a <see cref="BadRequestException"/>, indicating that
        /// the current exception occurred because of a bad client request.
        /// </summary>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param> 
        /// <returns>A new <see cref="BadRequestException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operationStackTrace"/> is <c>null</c>.
        /// </exception>
        protected virtual BadRequestException ToBadRequestException(MicroProcessorOperationStackTrace operationStackTrace) =>
            new BadRequestException(operationStackTrace, Message, this);

        /// <summary>
        /// Creates and returns this exception as a <see cref="InternalServerErrorException"/>, indicating that
        /// the current exception occurred because of an internal server error.        
        /// </summary>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param> 
        /// <returns>A new <see cref="InternalServerErrorException"/> with its <see cref="Exception.InnerException"/> set to this instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operationStackTrace"/> is <c>null</c>.
        /// </exception>
        protected virtual InternalServerErrorException ToInternalServerErrorException(MicroProcessorOperationStackTrace operationStackTrace) =>
            new InternalServerErrorException(operationStackTrace, Message, this);
    }
}
