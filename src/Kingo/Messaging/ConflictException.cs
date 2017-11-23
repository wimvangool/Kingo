using System;
using System.Runtime.Serialization;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when a command or query failed because a concurreny
    /// exception occurred while saving all changes. This type semantically maps to HTTP response code <c>409</c>.
    /// </summary>
    [Serializable]
    public class ConflictException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>        
        public ConflictException(object failedMessage) :
            base(failedMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>        
        public ConflictException(object failedMessage, string message)
            : base(failedMessage, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        public ConflictException(object failedMessage, string message, Exception innerException)
            : base(failedMessage, message, innerException) { }

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
