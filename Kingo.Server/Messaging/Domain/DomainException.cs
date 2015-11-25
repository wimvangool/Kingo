using System;
using System.Runtime.Serialization;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// This type of exception is thrown when something goes wrong in the domain of an application.
    /// </summary>    
    [Serializable]
    public abstract class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        protected DomainException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        protected DomainException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        protected DomainException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region [====== AsInvalidMessageException ======]

        /// <summary>
        /// Converts this instance into an instance of <see cref="InvalidMessageException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <returns>
        /// An instance of <see cref="InvalidMessageException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public InvalidMessageException AsInvalidMessageException(object failedMessage)
        {            
            var messageFormat = ExceptionMessages.DomainModelException_CommandFailed;
            var message = string.Format(messageFormat, failedMessage.GetType());
            return AsInvalidMessageException(failedMessage, message);
        }

        /// <summary>
        /// Converts this instance into an instance of <see cref="InvalidMessageException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <param name="message">Message of the exception.</param>
        /// <returns>
        /// An instance of <see cref="InvalidMessageException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public InvalidMessageException AsInvalidMessageException(object failedMessage, string message)
        {                     
            return AsInvalidMessageException(failedMessage, message, this);
        }

        /// <summary>
        /// Converts this instance into an instance of <see cref="InvalidMessageException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">This <see cref="DomainException" />.</param>
        /// <returns>
        /// An instance of <see cref="InvalidMessageException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        protected virtual InvalidMessageException AsInvalidMessageException(object failedMessage, string message, Exception innerException)
        {
            return new InvalidMessageException(failedMessage, message, innerException);
        }

        #endregion

        #region [====== AsCommandExecutionException ======]

        /// <summary>
        /// Converts this instance into an instance of <see cref="CommandExecutionException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <returns>
        /// An instance of <see cref="CommandExecutionException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException AsCommandExecutionException(object failedMessage)
        {            
            var messageFormat = ExceptionMessages.DomainModelException_CommandFailed;
            var message = string.Format(messageFormat, failedMessage.GetType());
            return new CommandExecutionException(failedMessage, message, this);
        }

        /// <summary>
        /// Converts this instance into an instance of <see cref="CommandExecutionException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <param name="message">Message of the exception.</param>
        /// <returns>
        /// An instance of <see cref="CommandExecutionException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException AsCommandExecutionException(object failedMessage, string message)
        {                        
            return AsCommandExecutionException(failedMessage, message, this);
        }

        /// <summary>
        /// Converts this instance into an instance of <see cref="CommandExecutionException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">This <see cref="DomainException" />.</param>
        /// <returns>
        /// An instance of <see cref="CommandExecutionException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        protected virtual CommandExecutionException AsCommandExecutionException(object failedMessage, string message, Exception innerException)
        {
            return new CommandExecutionException(failedMessage, message, innerException);
        }

        #endregion
    }
}
