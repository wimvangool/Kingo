using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This type of exception is thrown when an attempted operation is not allowed by the application logic.
    /// </summary>    
    [Serializable]
    public class BusinessRuleException : MessageHandlerOperationException
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        public BusinessRuleException(string message = null, Exception innerException = null) :
            base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public BusinessRuleException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <inheritdoc />
        public override BadRequestException ToBadRequestException(string message, MicroProcessorOperationStackTrace operationStackTrace = null) =>
            new UnprocessableEntityException(message, this, operationStackTrace);
    }
}
