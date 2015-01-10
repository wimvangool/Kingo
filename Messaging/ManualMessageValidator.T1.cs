namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{TMessage}" /> that validates a message by inline validation-logic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public class ManualMessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class
    {
        /// <inheritdoc />
        public bool TryGetValidationErrors(TMessage message, out ValidationErrorTree errorTree)
        {
            if (message == null)
            {
                errorTree = null;
                return false;
            }
            return Validate(message).TryBuildErrorTree(out errorTree);
        }

        /// <summary>
        /// Creates and returns a <see cref="ValidationErrorTreeBuilder" /> that contains all validation-errors
        /// of the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <returns>A <see cref="ValidationErrorTreeBuilder" /> that contains all validation-errors.</returns>
        protected virtual ValidationErrorTreeBuilder Validate(TMessage message)
        {
            return new ValidationErrorTreeBuilder(message.GetType());
        }
    }
}
