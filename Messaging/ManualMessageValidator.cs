using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> in which validation-errors are reported
    /// through a <see cref="ValidationErrorTreeBuilder" />.
    /// </summary>    
    public sealed class ManualMessageValidator : IMessageValidator
    {
        private readonly object _message;
        private readonly Action<ValidationErrorTreeBuilder> _validateMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualMessageValidator" /> class.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <param name="validateMethod">
        /// The delegate that is used to populate a <see cref="ValidationErrorTreeBuilder" /> for the specified <paramref name="message"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="validateMethod"/> is <c>null</c>.
        /// </exception>
        public ManualMessageValidator(object message, Action<ValidationErrorTreeBuilder> validateMethod)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (validateMethod == null)
            {
                throw new ArgumentNullException("validateMethod");
            }
            _message = message;
            _validateMethod = validateMethod;
        }

        /// <inheritdoc />
        public ValidationErrorTree Validate()
        {
            var builder = new ValidationErrorTreeBuilder(_message);

            _validateMethod.Invoke(builder);

            return builder.BuildErrorTree();
        }
    }
}
