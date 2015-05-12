using System.Runtime.Serialization;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// This exception is thrown when a business rule has been violated while processing a certain message.
    /// </summary>
    [Serializable]
    public class BusinessRuleViolationException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        public BusinessRuleViolationException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>        
        public BusinessRuleViolationException(string message)
            : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        public BusinessRuleViolationException(string message, Exception innerException)
            : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected BusinessRuleViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}
