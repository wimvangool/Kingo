﻿using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace YellowFlare.MessageProcessing
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
        protected DomainException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        protected DomainException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of the exception.</param>
        protected DomainException(string message, Exception inner) : base(message, inner) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        /// <summary>
        /// Converts this instance into an instance of <see cref="InvalidCommandException" />.
        /// </summary>
        /// <param name="command">The command that caused the exception.</param>
        /// <returns>
        /// An instance of <see cref="InvalidCommandException" /> that wraps this exception and the inner
        /// exception.
        /// </returns>
        public virtual InvalidCommandException AsInvalidCommandException(object command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            var messageFormat = ExceptionMessages.DomainException_CommandFailed;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, command.GetType());
            return new InvalidCommandException(command, message, this);
        }
    }
}
