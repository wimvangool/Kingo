namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{TMessage}" /> that validates a message by inline validation-logic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public sealed class ManualMessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class
    {
        private readonly Func<TMessage, ValidationErrorTreeBuilder> _builderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualMessageValidator{TMessage}" /> class.
        /// </summary>
        /// <param name="builderFactory">The method that is used to validate the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="builderFactory"/> is <c>null</c>.
        /// </exception>
        public ManualMessageValidator(Func<TMessage, ValidationErrorTreeBuilder> builderFactory)
        {
            if (builderFactory == null)
            {
                throw new ArgumentNullException("builderFactory");
            }
            _builderFactory = builderFactory;
        }

        /// <inheritdoc />
        public bool TryGetValidationErrors(TMessage message, out ValidationErrorTree errorTree)
        {
            if (message == null)
            {
                errorTree = null;
                return false;
            }
            return _builderFactory.Invoke(message).TryBuildErrorTree(out errorTree);
        }        
    }
}
