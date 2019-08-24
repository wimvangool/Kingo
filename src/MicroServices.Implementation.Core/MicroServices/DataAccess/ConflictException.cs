using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when a command or query failed because a concurrency
    /// exception occurred while saving all changes. This type semantically maps to HTTP response code <c>409</c> (conflict).
    /// </summary>
    [Serializable]
    public class ConflictException : BadRequestException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>        
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        public ConflictException(string message = null, Exception innerException = null)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Returns <c>409</c>.
        /// </summary>
        public override int ErrorCode =>
            409;
    }
}
