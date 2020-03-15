using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a service when a command or query failed to execute because it was
    /// invalid or because the operation was illegal. This type semantically maps to HTTP response code <c>400</c>.
    /// </summary>
    [Serializable]
    public class BadRequestException : MicroProcessorOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param> 
        public BadRequestException(string message = null, MicroProcessorOperationStackTrace operationStackTrace = null) :
            base(message, null, operationStackTrace) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param> 
        public BadRequestException(string message = null, Exception innerException = null, MicroProcessorOperationStackTrace operationStackTrace = null) :
            base(message, innerException, operationStackTrace) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected BadRequestException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <summary>
        /// Returns a value between <c>400</c> and <c>499</c>.
        /// </summary>
        public override int ErrorCode =>
            400;
    }
}
