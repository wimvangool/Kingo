using System;
using System.Runtime.Serialization;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// This type of exception is thrown when something goes wrong in the domain of an application.
    /// </summary>    
    [Serializable]
    public class DomainException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public DomainException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public DomainException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region [====== CreateException ======]

        /// <summary>
        /// Formats the specified <paramref name="messageFormat"/> with the specified argument
        /// and creates and returns a new <see cref="DomainException" /> with the resulting message.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="argument">The message argument.</param>
        /// <returns>
        /// A new <see cref="DomainException"/> with the specified message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="messageFormat"/> is not a valid format string.
        /// </exception>
        public static DomainException CreateException(string messageFormat, object argument)
        {
            return new DomainException(string.Format(messageFormat, argument));
        }

        /// <summary>
        /// Formats the specified <paramref name="messageFormat"/> with the specified arguments
        /// and creates and returns a new <see cref="DomainException" /> with the resulting message.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="argumentA">The first message argument.</param>
        /// <param name="argumentB">The second message argument.</param>
        /// <returns>
        /// A new <see cref="DomainException"/> with the specified message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="messageFormat"/> is not a valid format string.
        /// </exception>
        public static DomainException CreateException(string messageFormat, object argumentA, object argumentB)
        {
            return new DomainException(string.Format(messageFormat, argumentA, argumentB));
        }

        /// <summary>
        /// Formats the specified <paramref name="messageFormat"/> with the specified arguments
        /// and creates and returns a new <see cref="DomainException" /> with the resulting message.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="argumentA">The first message argument.</param>
        /// <param name="argumentB">The second message argument.</param>
        /// <param name="argumentC">The third message argument.</param>
        /// <returns>
        /// A new <see cref="DomainException"/> with the specified message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="messageFormat"/> is not a valid format string.
        /// </exception>
        public static DomainException CreateException(string messageFormat, object argumentA, object argumentB, object argumentC)
        {
            return new DomainException(string.Format(messageFormat, argumentA, argumentB, argumentC));
        }

        /// <summary>
        /// Formats the specified <paramref name="messageFormat"/> with the specified arguments
        /// and creates and returns a new <see cref="DomainException" /> with the resulting message.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arguments">The message arguments.</param>
        /// <returns>
        /// A new <see cref="DomainException"/> with the specified message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageFormat"/> or <paramref name="arguments"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="messageFormat"/> is not a valid format string.
        /// </exception>
        public static DomainException CreateException(string messageFormat, params object[] arguments)
        {
            return new DomainException(string.Format(messageFormat, arguments));
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
            return AsCommandExecutionException(failedMessage, message, this);
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

        #region [====== AsTechnicalException ======]

        /// <summary>
        /// Converts this instance into an instance of <see cref="TechnicalException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <returns>
        /// An instance of <see cref="TechnicalException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public TechnicalException AsTechnicalException(object failedMessage)
        {
            var messageFormat = ExceptionMessages.DomainModelException_EventFailed;
            var message = string.Format(messageFormat, failedMessage.GetType());
            return AsTechnicalException(failedMessage, message, this);
        }

        /// <summary>
        /// Converts this instance into an instance of <see cref="TechnicalException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <param name="message">Message of the exception.</param>
        /// <returns>
        /// An instance of <see cref="TechnicalException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        public TechnicalException AsTechnicalException(object failedMessage, string message)
        {
            return AsTechnicalException(failedMessage, message, this);
        }

        /// <summary>
        /// Converts this instance into an instance of <see cref="TechnicalException" />.
        /// </summary>
        /// <param name="failedMessage">The message that caused the exception.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">This <see cref="DomainException" />.</param>
        /// <returns>
        /// An instance of <see cref="TechnicalException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        protected virtual TechnicalException AsTechnicalException(object failedMessage, string message, Exception innerException)
        {
            return new TechnicalException(failedMessage, message, innerException);
        }

        #endregion
    }
}
