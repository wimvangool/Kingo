namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{TMessage}" /> that validates a message by inline validation-logic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public sealed class ManualMessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class
    {
        private readonly Func<TMessage, ValidationErrorTreeBuilder> _validateMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualMessageValidator{TMessage}" /> class.
        /// </summary>
        /// <param name="validateMethod">The method that is used to validate the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validateMethod"/> is <c>null</c>.
        /// </exception>
        public ManualMessageValidator(Func<TMessage, ValidationErrorTreeBuilder> validateMethod)
        {
            if (validateMethod == null)
            {
                throw new ArgumentNullException("validateMethod");
            }
            _validateMethod = validateMethod;
        }

        /// <inheritdoc />
        public bool TryGetValidationErrors(TMessage message, out ValidationErrorTree errorTree)
        {
            if (message == null)
            {
                errorTree = null;
                return false;
            }
            return _validateMethod.Invoke(message).TryBuildErrorTree(out errorTree);
        }        
    }
}
