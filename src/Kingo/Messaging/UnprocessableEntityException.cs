using System;
using System.Runtime.Serialization;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when a command failed to execute because because the operation was illegal.
    /// This type semantically maps to HTTP response code <c>422</c>.
    /// </summary>
    [Serializable]
    public class UnprocessableEntityException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>        
        public UnprocessableEntityException(object failedMessage)
            : base(failedMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>        
        public UnprocessableEntityException(object failedMessage, string message)
            : base(failedMessage, message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        public UnprocessableEntityException(object failedMessage, string message, Exception innerException)
            : base(failedMessage, message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnprocessableEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
