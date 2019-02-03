using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when a command or query failed because the client
    /// was not authorized to execute it. This type semantically maps to HTTP response code <c>401</c>.
    /// </summary>
    [Serializable]
    public class UnauthorizedRequestException : BadRequestException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedRequestException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        public UnauthorizedRequestException(string message = null, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnauthorizedRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Returns <c>401</c>.
        /// </summary>
        public override int ErrorCode =>
            401;
    }
}
