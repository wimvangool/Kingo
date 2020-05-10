using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a processor when a command or query failed because the client
    /// was not authorized to execute it. This type semantically maps to HTTP response code <c>401</c>.
    /// </summary>
    [Serializable]
    public class UnauthorizedRequestException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedRequestException" /> class.
        /// </summary>
        /// <param name="operationStackTrace">The stack trace of the processor at the time the exception was thrown.</param>  
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operationStackTrace"/> is <c>null</c>.
        /// </exception>
        public UnauthorizedRequestException(MicroProcessorOperationStackTrace operationStackTrace, string message = null, Exception innerException = null) :
            base(operationStackTrace, message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected UnauthorizedRequestException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <summary>
        /// Returns <c>401</c>.
        /// </summary>
        public override int ErrorCode =>
            401;
    }
}
