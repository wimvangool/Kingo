using System;
using System.Runtime.Serialization;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// This type of exception is thrown when an attempted operation in the domain of the application is not allowed.
    /// </summary>    
    [Serializable]
    public class IllegalOperationException : InternalProcessorException
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalOperationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public IllegalOperationException(string message) :
            base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalOperationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        public IllegalOperationException(string message, Exception innerException) :
            base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalOperationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public IllegalOperationException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <inheritdoc />
        public override BadRequestException AsBadRequestException(object failedMessage, string message) =>
            new UnprocessableEntityException(failedMessage, message, this);
    }
}
