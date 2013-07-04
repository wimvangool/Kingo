using System;
using System.Globalization;
using System.Runtime.Serialization;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// This type of exception is thrown when something goes wrong in the domain of an application.
    /// </summary>    
    [Serializable]
    public abstract class AggregatesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesException" /> class.
        /// </summary>
        protected AggregatesException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        protected AggregatesException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of the exception.</param>
        protected AggregatesException(string message, Exception inner) : base(message, inner) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatesException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected AggregatesException(SerializationInfo info, StreamingContext context) : base(info, context) {}

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
            var message = Format(messageFormat, command.GetType());
            return new InvalidCommandException(command, message, this);
        }

        /// <summary>
        /// Formats the specified message using the current culture.
        /// </summary>
        /// <param name="messageFormat">Format-string of the message.</param>
        /// <param name="a">First argument to use in the format-string.</param>
        /// <returns>The formatted string.</returns>
        protected static string Format(string messageFormat, object a)
        {
            return string.Format(CultureInfo.CurrentCulture, messageFormat, a);
        }

        /// <summary>
        /// Formats the specified message using the current culture.
        /// </summary>
        /// <param name="messageFormat">Format-string of the message.</param>
        /// <param name="a">First argument to use in the format-string.</param>
        /// <param name="b">Second argument to use in the format-string.</param>
        /// <returns>The formatted string.</returns>
        protected static string Format(string messageFormat, object a, object b)
        {
            return string.Format(CultureInfo.CurrentCulture, messageFormat, a, b);
        }

        /// <summary>
        /// Formats the specified message using the current culture.
        /// </summary>
        /// <param name="messageFormat">Format-string of the message.</param>
        /// <param name="a">First argument to use in the format-string.</param>
        /// <param name="b">Second argument to use in the format-string.</param>
        /// <param name="c">Third argument to use in the format-string.</param>
        /// <returns>The formatted string.</returns>
        protected static string Format(string messageFormat, object a, object b, object c)
        {
            return string.Format(CultureInfo.CurrentCulture, messageFormat, a, b, c);
        }

        /// <summary>
        /// Formats the specified message using the current culture.
        /// </summary>
        /// <param name="messageFormat">Format-string of the message.</param>
        /// <param name="parameters">All parameters to use in the format-string.</param>
        /// <returns>The formatted string.</returns>
        protected static string Format(string messageFormat, params object[] parameters)
        {
            return string.Format(CultureInfo.CurrentCulture, messageFormat, parameters);
        }
    }
}
