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
            InvalidMessageException exception;

            if (TryCreateInvalidMessageException(handler.Message, out exception))
            {
                throw exception;               
            }            
            handler.Invoke();
        }

        /// <summary>
        /// Validates the specified <paramref name="message"/> and creates a new <see cref="InvalidMessageException" />
        /// containing all validation errors if any valdation errors were detected.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <param name="exception">
        /// If <paramref name="message"/> has any validation errors, this parameter will refer to a new
        /// <see cref="InvalidMessageException"/> instance.
        /// </param>
        /// <returns><c>true</c> if any validation errors were detected; otherwise <c>false</c>.</returns>
        protected static bool TryCreateInvalidMessageException(IMessage message, out InvalidMessageException exception)
        {
            var errorTree = message.Validate();
            if (errorTree.TotalErrorCount == 0)
            {
                exception = null;
                return false;
            }
            exception = new InvalidMessageException(message, ExceptionMessages.InvalidMessageException_InvalidMessage, errorTree);
            return true;
        }
    }
}
