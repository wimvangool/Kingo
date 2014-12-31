using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// This exception is thrown when a business rule was violated while processing a <see cref="IMessage" />.
    /// </summary>
    [Serializable]
    public class BusinessRuleViolationException : FunctionalException
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public BusinessRuleViolationException(IMessage failedMessage)
            : base(failedMessage) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public BusinessRuleViolationException(IMessage failedMessage, string message)
            : base(failedMessage, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public BusinessRuleViolationException(IMessage failedMessage, string message, Exception inner)
            : base(failedMessage, message, inner) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected BusinessRuleViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}
