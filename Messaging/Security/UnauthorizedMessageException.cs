using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Security
{
    /// <summary>
    /// This exception is thrown when the current user, typically the identity of the sender of a message,
    /// has insufficient rights to process a specific message.
    /// </summary>
    [Serializable]
    public class UnauthorizedMessageException : FunctionalException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedMessageException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public UnauthorizedMessageException(IMessage failedMessage) 
            : base(failedMessage) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public UnauthorizedMessageException(IMessage failedMessage, string message)
            : base(failedMessage, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public UnauthorizedMessageException(IMessage failedMessage, string message, Exception innerException)
            : base(failedMessage, message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected UnauthorizedMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}
