using System;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a pipeline that validates all messages going through and throws an <see cref="InvalidMessageException" /> when
    /// a message contains valiation errors.
    /// </summary>
    public class MessageValidationPipeline : MicroProcessorPipeline
    {
        /// <summary>
        /// Validates the message and throws an <see cref="InvalidMessageException" /> is any validation errors are found.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="InvalidMessageException">
        /// The specified message is invalid.
        /// </exception>
        protected override async Task<TResult> HandleOrExecuteAsync<TResult>(MessageHandlerOrQuery<TResult> handlerOrQuery, IMicroProcessorContext context)
        {
            var message = context.Messages.Current.Message as IMessage;
            if (message != null)
            {
                var errorInfo = message.Validate(true);
                if (errorInfo.HasErrors)
                {
                    throw NewInvalidMessageException(message.GetType(), errorInfo);
                }
            }
            return await handlerOrQuery.HandleMessageOrExecuteQueryAsync(context);
        }  
        
        private static InvalidMessageException NewInvalidMessageException(Type messageType, ErrorInfo errorInfo)
        {
            var messageFormat = ExceptionMessages.MessageValidationPipeline_InvalidMessage;
            var message = string.Format(messageFormat, messageType.FriendlyName(), errorInfo.ErrorCount);
            return new InvalidMessageException(errorInfo, message);
        }
    }
}
