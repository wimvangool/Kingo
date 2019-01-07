using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Validation
{
    /// <summary>
    /// Represents a filter that validates all messages going through and throws an <see cref="InvalidRequestException" /> when
    /// a message contains valiation errors.
    /// </summary>
    public sealed class ValidateRequestAttribute : ValidationFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateRequestAttribute" /> class.
        /// </summary>       
        public ValidateRequestAttribute(bool haltOnFirstError = true)           
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
        /// <returns>The result of the operation.</returns>
        /// <exception cref="InvalidRequestException">
        /// The specified message is invalid.
        /// </exception>
        protected override async Task<InvokeAsyncResult<TResult>> InvokeAsync<TResult>(IMessageHandlerOrQuery<TResult> handlerOrQuery)
        {            
            if (handlerOrQuery.Context.Operation.Message is IRequestMessage requestMessage)
            {
                var errorInfo = requestMessage.Validate(HaltOnFirstError);
                if (errorInfo.HasErrors)
                {
                    throw NewInvalidMessageException(requestMessage.GetType(), errorInfo);
                }
            }
            return await handlerOrQuery.Method.InvokeAsync().ConfigureAwait(false);
        }                 

        private static InvalidRequestException NewInvalidMessageException(Type messageType, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.MessageValidationPipeline_InvalidMessage;
            var message = string.Format(messageFormat, messageType.FriendlyName(), errorInfo);
            return new InvalidRequestException(errorInfo, message);
        }
    }
}
