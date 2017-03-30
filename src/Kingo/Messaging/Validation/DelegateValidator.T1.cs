using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}"/> that is implemented through a delegate.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that can be validated by this validator.</typeparam>
    public sealed class DelegateValidator<TMessage> : IMessageValidator<TMessage>
    {
        private readonly Func<TMessage, bool, ErrorInfo> _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateValidator{T}" /> class.
        /// </summary>
        /// <param name="implementation">The delegate that will be used to validate each message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementation"/> is <c>null</c>.
        /// </exception>
        public DelegateValidator(Func<TMessage, bool, ErrorInfo> implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }
            _implementation = implementation;
        }

        /// <inheritdoc />
        public ErrorInfo Validate(TMessage message, bool haltOnFirstError = false) =>
            _implementation.Invoke(message, haltOnFirstError);
    }
}
