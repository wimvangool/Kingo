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
        /// <param name="innerException">Cause of this exception.</param>        
        public BadRequestException(string message = null, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected BadRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Returns a value between <c>400</c> and <c>499</c>.
        /// </summary>
        public override int ErrorCode =>
            400;
    }
}
