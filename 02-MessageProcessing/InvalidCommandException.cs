using System;
using System.Runtime.Serialization;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// This exception or any derived type is thrown is a command is considered invalid due to its value,
    /// the state of the application or a combination of both.
    /// </summary>
    [Serializable]
    public class InvalidCommandException : Exception
    {
        private readonly object _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="command">The invalid command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public InvalidCommandException(object command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="command">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public InvalidCommandException(object command, string message) : base(message)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="command">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public InvalidCommandException(object command, string message, Exception inner) : base(message, inner)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected InvalidCommandException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        /// <summary>
        /// The invalid command.
        /// </summary>
        public object Command
        {
            get { return _command; }
        }
    }
}
