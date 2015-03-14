using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidationStrategy" /> in which validation-errors are reported
    /// through the built in <see cref="Validator" /> that uses <see cref="ValidationAttribute">ValidationAttributes</see>
    /// as a source of validation-logic.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public sealed class AutomaticValidationStrategy<TMessage> : IMessageValidationStrategy where TMessage : class
    {
        private readonly TMessage _message;
        private readonly AutomaticMessageValidator<TMessage> _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticValidationStrategy{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public AutomaticValidationStrategy(TMessage message)
            : this(message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticValidationStrategy{TMessage}" /> class.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <param name="validationContextFactory">
        /// Optional factory to create a custom <see cref="ValidationContext" /> for the <paramref name="message"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public AutomaticValidationStrategy(TMessage message, Func<TMessage, ValidationContext> validationContextFactory)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;
            _validator = new AutomaticMessageValidator<TMessage>(validationContextFactory);
        }

        /// <inheritdoc />
        public bool TryGetValidationErrors(out ValidationErrorTree errorTree)
        {
            return _validator.TryGetValidationErrors(_message, out errorTree);
        }
    }
}
