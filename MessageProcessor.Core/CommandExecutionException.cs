using System;
using System.Runtime.Serialization;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// This exception or any derived type is thrown if a command is considered invalid, could
    /// not be executed because of insufficient rights or caused a certain business rule violation.
    /// </summary>
    [Serializable]
    public class CommandExecutionException : Exception
    {
        private readonly object _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="command">The invalid command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException(object command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="command">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException(object command, string message) : base(message)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="command">The invalid command.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionException(object command, string message, Exception inner) : base(message, inner)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected CommandExecutionException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        /// <summary>
        /// The invalid command.
        /// </summary>
        public object Command
        {
            get { return _command; }
        }
    }
}
