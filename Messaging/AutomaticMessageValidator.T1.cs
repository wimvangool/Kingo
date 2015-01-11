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
    public sealed class AutomaticMessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class
    {
        private readonly Func<TMessage, ValidationContext> _validationContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticMessageValidator{TMessage}" /> class.
        /// </summary>        
        public AutomaticMessageValidator()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticMessageValidator{TMessage}" /> class.
        /// </summary>
        /// <param name="validationContextFactory">
        /// The method used to create a <see cref="ValidationContext" /> for a specific message.
        /// </param>        
        public AutomaticMessageValidator(Func<TMessage, ValidationContext> validationContextFactory)
        {            
            _validationContextFactory = validationContextFactory ?? (message => new ValidationContext(message, null, null));
        }

        /// <inheritdoc />
        public bool TryGetValidationErrors(TMessage message, out ValidationErrorTree errorTree)
        {
            if (message == null)
            {
                errorTree = null;
                return false;
            }
            return TryGetValidationErrors(_validationContextFactory.Invoke(message), out errorTree);
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
                errorMessagesPerMember.Add(member.Key, ValidationErrorTree.Concatenate(member.Value));
            }
            return errorMessagesPerMember;
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
