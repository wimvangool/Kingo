using System;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a filter that validates all messages going through and throws an <see cref="InvalidRequestException" /> when
    /// a message contains valiation errors.
    /// </summary>
    public sealed class ValidateRequestMessageAttribute : ValidationFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateRequestMessageAttribute" /> class.
        /// </summary>       
        public ValidateRequestMessageAttribute(bool haltOnFirstError = true)           
        {
            HaltOnFirstError = haltOnFirstError;
        }

        /// <summary>
        /// Indicates whether or not every validation should halt on the first validation-error.
        /// </summary>
        public bool HaltOnFirstError
        {
            get;
        }

        /// <summary>
        /// Validates the message and throws an <see cref="InvalidRequestException" /> is any validation errors are found.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="InvalidRequestException">
        /// The specified message is invalid.
        /// </exception>
        protected override async Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context)
        {
            var messageToValidate = context.Messages.Current?.Message;
            if (messageToValidate != null)
            {
                var errorInfo = Validate(messageToValidate);
                if (errorInfo.HasErrors)
                {
                    throw NewInvalidMessageException(messageToValidate.GetType(), errorInfo);
                }
            }
            return await handlerOrQuery.HandleMessageOrExecuteQueryAsync(context);
        }  

        private ErrorInfo Validate(object messageToValidate)
        {
            if (messageToValidate is IRequestMessage message)
            {
                return message.Validate(HaltOnFirstError);
            }
            IRequestMessageValidator validator;

            if (RequestMessageBase.TryGetMessageValidator(messageToValidate.GetType(), out validator))
            {
                return validator.Validate(messageToValidate, HaltOnFirstError);
            }
            return ErrorInfo.Empty;
        }        

        private static InvalidRequestException NewInvalidMessageException(Type messageType, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.MessageValidationPipeline_InvalidMessage;
            var message = string.Format(messageFormat, messageType.FriendlyName(), errorInfo);
            return new InvalidRequestException(errorInfo, message);
        }
    }
}
