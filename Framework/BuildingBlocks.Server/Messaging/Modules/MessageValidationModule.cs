using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Modules
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
        public override async Task InvokeAsync(IMessageHandler handler)
        {                   
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var errorInfoCollection = await Validate(handler.Message);
            if (errorInfoCollection.Count > 0)
            {
                throw NewInvalidMessageException(handler.Message, errorInfoCollection);
            }
            await handler.InvokeAsync();
        }

        /// <summary>
        /// Validates the specified <paramref name="message" /> and returns the resulting <see cref="DataErrorInfo" />.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <returns>The resulting <see cref="DataErrorInfo" />.</returns>
        protected virtual Task<IReadOnlyList<DataErrorInfo>> Validate(IMessage message)
        {
            return AsyncMethod.RunSynchronously(() => message.Validate());
        }        

        /// <summary>
        /// Creates and returns a new <see cref="InvalidMessageException"/> indicating that the specified
        /// <paramref name="message"/> is not valid.
        /// </summary>
        /// <param name="message">The invalid message.</param>
        /// <param name="errorInfoCollection">A collection of <see cref="DataErrorInfo" /> instances containing all validation errors.</param>
        /// <returns>A new <see cref="InvalidMessageException"/>.</returns>
        protected virtual InvalidMessageException NewInvalidMessageException(IMessage message, IReadOnlyList<DataErrorInfo> errorInfoCollection)
        {
            return new InvalidMessageException(message, ExceptionMessages.InvalidMessageException_InvalidMessage, errorInfoCollection);
        }
    }
}
