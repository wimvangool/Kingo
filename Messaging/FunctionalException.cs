using System.ComponentModel.Server;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// This <see cref="Exception" /> is thrown when a message could not be processed by a <see cref="IMessageProcessor" />
    /// for functional reasons, meaning that the (sender of the) message did not meet the preconditions for correct processing of this message.    
    /// </summary>
    [Serializable]
    public abstract class FunctionalException : Exception
    {
        private readonly object _failedMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        protected FunctionalException(object failedMessage)
        {
            if (failedMessage == null)
            {
                throw new ArgumentNullException("failedMessage");
            }
            _failedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        protected FunctionalException(object failedMessage, string message)
            : base(message)
        {
            if (failedMessage == null)
            {
                throw new ArgumentNullException("failedMessage");
            }
            _failedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="inner">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        protected FunctionalException(object failedMessage, string message, Exception inner)
            : base(message, inner)
        {
            if (failedMessage == null)
            {
                throw new ArgumentNullException("failedMessage");
            }
            _failedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected FunctionalException(SerializationInfo info, StreamingContext context)
            : base(info, context) { /* TODO */ }

        /// <summary>
        /// The message that could not be processed.
        /// </summary>
        public object FailedMessage
        {
            get { return _failedMessage; }
        }
    }
}
