using System;
using System.Runtime.Serialization;

namespace ServiceComponents.ComponentModel
{
    /// <summary>
    /// This exception is thrown when a business rule was violated while processing a command.
    /// </summary>
    [Serializable]
    public class CommandExecutionException : FunctionalException
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException(IMessage failedMessage)
            : base(failedMessage) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException(IMessage failedMessage, string message)
            : base(failedMessage, message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException(IMessage failedMessage, string message, Exception innerException)
            : base(failedMessage, message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected CommandExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}
