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
    public abstract class ExternalProcessorException : Exception
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalProcessorException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>        
        internal ExternalProcessorException(object failedMessage)
        {            
            FailedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalProcessorException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>        
        internal ExternalProcessorException(object failedMessage, string message)
            : base(message)
        {            
            FailedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalProcessorException" /> class.
        /// </summary>
        /// <param name="failedMessage">The message that could not be processed.</param>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of this exception.</param>        
        internal ExternalProcessorException(object failedMessage, string message, Exception innerException)
            : base(message, innerException)
        {            
            FailedMessage = failedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalProcessorException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal ExternalProcessorException(SerializationInfo info, StreamingContext context)
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

        /// <summary>
        /// The ErrorCode that is associated with this Exception. This code typically corresponds with HttpStatusCodes,
        /// which implies the returned value is always somewhere between <c>400</c> and <c>599</c>, depending on the
        /// run-time type of the exception.
        /// </summary>
        public abstract int ErrorCode
        {
            get;            
        }
    }
}
