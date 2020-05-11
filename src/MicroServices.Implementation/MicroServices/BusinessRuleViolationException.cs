using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This type of exception is thrown when an attempted operation is not allowed by the application logic.
    /// </summary>    
    [Serializable]
    public class BusinessRuleViolationException : InternalOperationException
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Cause of the exception.</param>
        public BusinessRuleViolationException(string message = null, Exception innerException = null) :
            base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessRuleViolationException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public BusinessRuleViolationException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        /// <inheritdoc />
        protected override bool IsBadRequest(MicroProcessorOperationStackTrace operationStackTrace) =>
            IsCommandOperationStackTrace(operationStackTrace);

        internal static bool IsCommandOperationStackTrace(MicroProcessorOperationStackTrace operationStackTrace) =>
            operationStackTrace.All(IsCommandOperation);

        private static bool IsCommandOperation(MicroProcessorOperationStackItem operation) =>
            IsCommand(operation.Message);

        private static bool IsCommand(IMessage message) =>
            message != null && message.Kind == MessageKind.Command;

        /// <inheritdoc />
        protected override BadRequestException ToBadRequestException(MicroProcessorOperationStackTrace operationStackTrace) =>
            new UnprocessableEntityException(operationStackTrace, Message, this);
    }
}
