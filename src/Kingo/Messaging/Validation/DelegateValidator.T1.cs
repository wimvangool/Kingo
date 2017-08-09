using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a <see cref="IRequestMessageValidator{TMessage}"/> that is implemented through a delegate.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that can be validated by this validator.</typeparam>
    public sealed class DelegateValidator<TMessage> : IRequestMessageValidator<TMessage>
    {
        private readonly Func<TMessage, bool, ErrorInfo> _validationMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateValidator{T}" /> class.
        /// </summary>
        /// <param name="validationMethod">The delegate that will be used to validate each message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validationMethod"/> is <c>null</c>.
        /// </exception>
        public DelegateValidator(Func<TMessage, bool, ErrorInfo> validationMethod)
        {
            if (validationMethod == null)
            {
                throw new ArgumentNullException(nameof(validationMethod));
            }
            _validationMethod = validationMethod;
        }

        /// <inheritdoc />
        public ErrorInfo Validate(TMessage message, bool haltOnFirstError = false) =>
            _validationMethod.Invoke(message, haltOnFirstError);
    }
}
