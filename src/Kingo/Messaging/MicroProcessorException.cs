using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging
{
    /// <summary>
    /// This exception is thrown by a <see cref="IMicroProcessor" /> when it failed to handle a message or execute a query.
    /// Any derived type of this class semantically maps to a <c>4xx</c> or <c>5xx</c> HTTP status code.
    /// </summary>
    [Serializable]
    public abstract class MicroProcessorException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        internal MicroProcessorException(object failedMessage)
        {
            if (failedMessage == null)
            {
                throw new ArgumentNullException(nameof(failedMessage));
            }
            FailedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        internal MicroProcessorException(object failedMessage, string message)
            : base(message)
        {
            if (failedMessage == null)
            {
                throw new ArgumentNullException(nameof(failedMessage));
            }
            FailedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="failedMessage"/> is <c>null</c>.
        /// </exception>
        internal MicroProcessorException(object failedMessage, string message, Exception innerException)
            : base(message, innerException)
        {
            if (failedMessage == null)
            {
                throw new ArgumentNullException(nameof(failedMessage));
            }
            FailedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal MicroProcessorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            FailedMessage = info.GetValue(nameof(FailedMessage), typeof(object));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(FailedMessage), FailedMessage);
        }

        /// <summary>
        /// The message that could not be processed.
        /// </summary>
        public object FailedMessage
        {
            get;
        }
    }
}
