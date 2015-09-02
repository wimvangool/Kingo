using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// This exception is thrown when a message could not be processed by a <see cref="IMessageProcessor" />
    /// for functional reasons, meaning that the (sender of the) message did not meet the preconditions for correct processing of this message.    
    /// </summary>
    [Serializable]
    public abstract class FunctionalException : Exception
    {
        private const string _FailedMessageKey = "_failedMessage";
        private readonly IMessage _failedMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        internal FunctionalException(IMessage failedMessage)
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
        internal FunctionalException(IMessage failedMessage, string message)
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
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        internal FunctionalException(IMessage failedMessage, string message, Exception innerException)
            : base(message, innerException)
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
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal FunctionalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _failedMessage = (IMessage) info.GetValue(_FailedMessageKey, typeof(IMessage));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_FailedMessageKey, _failedMessage);
        }

        /// <summary>
        /// The message that could not be processed.
        /// </summary>
        public IMessage FailedMessage
        {
            get { return _failedMessage; }
        }
    }
}
