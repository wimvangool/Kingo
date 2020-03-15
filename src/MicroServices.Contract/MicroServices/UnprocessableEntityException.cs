using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a service when a command failed to execute because because the operation was illegal.
    /// This type semantically maps to HTTP response code <c>422</c> (unprocessable entity).
    /// </summary>
    [Serializable]
    public class UnprocessableEntityException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>  
        public UnprocessableEntityException(string message = null, MicroProcessorOperationStackTrace operationStackTrace = null) :
            base(message, operationStackTrace) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>  
        public UnprocessableEntityException(string message = null, Exception innerException = null, MicroProcessorOperationStackTrace operationStackTrace = null) :
            base(message, innerException, operationStackTrace) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnprocessableEntityException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <summary>
        /// Returns <c>422</c>.
        /// </summary>
        public override int ErrorCode =>
            422;
    }
}
