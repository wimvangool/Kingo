using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception can be thrown when an application's post-condition for handling a certain message failed.   
    /// </summary>
    [Serializable]
    public class TechnicalException : Exception
    {
        private const string _FailedMessageKey = "_failedMessage";
        private readonly object _failedMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        internal TechnicalException(object failedMessage)
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
        internal TechnicalException(object failedMessage, string message)
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
        internal TechnicalException(object failedMessage, string message, Exception innerException)
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
        internal TechnicalException(SerializationInfo info, StreamingContext context)
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
        public object FailedMessage
        {
            get { return _failedMessage; }
        }
    }
}
