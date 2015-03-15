using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> that validates a message through
    /// all <see cref="ValidationAttribute">ValidationAttributes</see> that have been declared on the
    /// members of a message.
    /// </summary>    
    public sealed class AutomaticMessageValidator : IMessageValidator
    {        
        private readonly Func<ValidationContext> _validationContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticMessageValidator" /> class.
        /// </summary> 
        /// <param name="message">The message to validate.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public AutomaticMessageValidator(object message)
            : this(message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticMessageValidator" /> class.
        /// </summary>        
        /// <param name="message">The message to validate.</param>       
        /// <param name="validationContextFactory">
        /// The method used to create a <see cref="ValidationContext" /> for a specific message.
        /// </param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>   
        public AutomaticMessageValidator(object message, Func<ValidationContext> validationContextFactory)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }           
            _validationContextFactory = validationContextFactory ?? (() => new ValidationContext(message, null, null));
        }

        /// <inheritdoc />
        public ValidationErrorTree Validate()
        {
            return Validate(_validationContextFactory.Invoke());
        }

        private ValidationErrorTree Validate(ValidationContext validationContext)
        {            
            RequestMessageLabelProvider.Add(validationContext.ObjectInstance);

            try
            {
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);
                if (isValid)
                {
                    return ValidationErrorTree.NoErrors(validationContext.ObjectInstance);
                }
                return new ValidationErrorTree(validationContext.ObjectInstance, CreateErrorMessagesPerMember(validationResults));                
            }
            finally
            {
                RequestMessageLabelProvider.Remove(validationContext.ObjectInstance);
            }
        }        

        private static Dictionary<string, string> CreateErrorMessagesPerMember(IEnumerable<ValidationResult> validationResults)
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
