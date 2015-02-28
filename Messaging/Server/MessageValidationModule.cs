using System.ComponentModel.Resources;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a module that validates a message and throws an <see cref="InvalidMessageException" /> if
    /// a message is invalid.
    /// </summary>   
    public class MessageValidationModule : MessageHandlerModule
    {        
        /// <summary>
        /// Validates a message before it invokes the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>               
        /// <exception cref="InvalidMessageException">
        /// <paramref name="handler"/> is invalid.
        /// </exception>
        protected override void InvokeHandler(IMessageHandler handler)
        {                        
            ValidationErrorTree errors;

            if (handler.Message.TryGetValidationErrors(out errors))
            {
                throw new InvalidMessageException(handler, ExceptionMessages.MessageProcessor_InvalidMessage, errors);                
            }            
            handler.Invoke();
        }
    }
}
