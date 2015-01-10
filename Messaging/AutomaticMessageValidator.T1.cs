using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{TMessage}" /> that validates a message through
    /// all <see cref="ValidationAttribute">ValidationAttributes</see> that have been declared on the
    /// members of a message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to validate.</typeparam>
    public class AutomaticMessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class
    {
        /// <inheritdoc />
        public bool TryGetValidationErrors(TMessage message, out ValidationErrorTree errorTree)
        {
            if (message == null)
            {
                errorTree = null;
                return false;
            }
            return TryGetValidationErrors(CreateValidationContext(message), out errorTree);
        }

        private bool TryGetValidationErrors(ValidationContext validationContext, out ValidationErrorTree errorTree)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }
            RequestMessageLabelProvider.Add(validationContext.ObjectInstance);

            try
            {
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);

                if (isValid)
                {
                    errorTree = null;
                    return false;
                }
                errorTree = new ValidationErrorTree(validationContext.ObjectType, CreateErrorMessagesPerMember(validationResults));
                return true;
            }
            finally
            {
                RequestMessageLabelProvider.Remove(validationContext.ObjectInstance);
            }
        }

        /// <summary>
        /// Creates and returns a <see cref="ValidationContext" /> instance for the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <returns>A new <see cref="ValidationContext" /> instance.</returns>
        protected virtual ValidationContext CreateValidationContext(TMessage message)
        {
            return new ValidationContext(message, null, null);
        }

        private Dictionary<string, string> CreateErrorMessagesPerMember(IEnumerable<ValidationResult> validationResults)
        {
            var errorMessageBuilder = new Dictionary<string, List<string>>();

            foreach (var result in validationResults)
            {
                AppendErrorMessage(errorMessageBuilder, result);
            }
            var errorMessagesPerMember = new Dictionary<string, string>();

            foreach (var member in errorMessageBuilder)
            {
                errorMessagesPerMember.Add(member.Key, Concatenate(member.Value));
            }
            return errorMessagesPerMember;
        }

        /// <summary>
        /// Concatenates all validation-errors found for a single member into a single error-message.
        /// </summary>
        /// <param name="errorMessagesForMember">A collection of validation-errors.</param>
        /// <returns>A concatenated string of error-messages.</returns>
        protected virtual string Concatenate(IEnumerable<string> errorMessagesForMember)
        {
            return ValidationErrorTree.Concatenate(errorMessagesForMember);
        }

        private static void AppendErrorMessage(IDictionary<string, List<string>> errorMessageBuilder, ValidationResult result)
        {
            foreach (var memberName in result.MemberNames)
            {
                List<string> errorMessages;

                if (!errorMessageBuilder.TryGetValue(memberName, out errorMessages))
                {
                    errorMessageBuilder.Add(memberName, errorMessages = new List<string>());
                }
                errorMessages.Add(result.ErrorMessage);
            }
        }
    }
}
