using System;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific message.
    /// </summary>
    public interface IMessageValidator<in TMessage>
    {
        /// <summary>
        /// Validates all values of the specified <paramref name="message"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="message">The message to validate.</param>             
        /// <returns>
        /// A <see cref="MessageErrorInfo" /> instance that contain the error messages for all invalid members.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>   
        MessageErrorInfo Validate(TMessage message); 
    }
}
