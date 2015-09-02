using System;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific message.
    /// </summary>
    public interface IMessageValidator<in TMessage> where TMessage : class
    {
        /// <summary>
        /// Validates all values of the specified <paramref name="message"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="message">The message to validate.</param>             
        /// <returns>A <see cref="DataErrorInfo" /> containing all validation-errors (if any).</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>   
        DataErrorInfo Validate(TMessage message); 
    }
}
