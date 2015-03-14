using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidationStrategy" /> in which validation-errors are reported
    /// through a <see cref="ValidationErrorTreeBuilder" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public sealed class ManualValidationStrategy<TMessage> : IMessageValidationStrategy where TMessage : class
    {
        private readonly TMessage _message;
        private readonly ManualMessageValidator<TMessage> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualValidationStrategy{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <param name="builderFactory">
        /// The delegate that is used to populate a <see cref="ValidationErrorTreeBuilder" /> for the specified <paramref name="message"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="builderFactory "/> is <c>null</c>.
        /// </exception>
        public ManualValidationStrategy(TMessage message, Action<ValidationErrorTreeBuilder> builderFactory)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (builderFactory == null)
            {
                throw new ArgumentNullException("builderFactory");
            }
            _message = message;
            _validator = new ManualMessageValidator<TMessage>(msg =>
            {
                var errorTreeBuilder = new ValidationErrorTreeBuilder(msg.GetType());
                builderFactory.Invoke(errorTreeBuilder);
                return errorTreeBuilder;
            });
        }

        /// <inheritdoc />
        public bool TryGetValidationErrors(out ValidationErrorTree errorTree)
        {
            return _validator.TryGetValidationErrors(_message, out errorTree);
        }
    }
}
