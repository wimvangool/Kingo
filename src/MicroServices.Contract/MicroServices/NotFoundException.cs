using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a processor when a query failed to execute because the
    /// requested data or resource was not found. This type semantically maps to HTTP response code <c>404</c>.
    /// </summary>
    [Serializable]
    public class NotFoundException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>  
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operationStackTrace"/> is <c>null</c>.
        /// </exception>
        public NotFoundException(MicroProcessorOperationStackTrace operationStackTrace, string message = null, Exception innerException = null) :
            base(operationStackTrace, message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NotFoundException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <summary>
        /// Returns <c>404</c>.
        /// </summary>
        public override int ErrorCode =>
            404;
    }
}
