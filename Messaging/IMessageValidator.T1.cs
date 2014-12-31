namespace System.ComponentModel
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific type of message.
    /// </summary>
    public interface IMessageValidator<in TMessage> where TMessage : class
    {
        /// <summary>
        /// Validates all values of a message and returns whether or not any errors were found.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, this parameter will contain all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.</returns>
        bool IsNotValid(TMessage message, out MessageErrorTree errorTree);
    }
}
