using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> that validates a message through
    /// all <see cref="ValidationAttribute">ValidationAttributes</see> that have been declared on the
    /// members of a message.
    /// </summary>    
    public sealed class MessageValidator : IMessageValidator
    {        
        private readonly Func<ValidationContext> _validationContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidator" /> class.
        /// </summary> 
        /// <param name="message">The message to validate.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageValidator(object message)
            : this(message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidator" /> class.
        /// </summary>        
        /// <param name="message">The message to validate.</param>       
        /// <param name="validationContextFactory">
        /// The method used to create a <see cref="ValidationContext" /> for a specific message.
        /// </param>     
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>   
        public MessageValidator(object message, Func<ValidationContext> validationContextFactory)
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

        private static ValidationErrorTree Validate(ValidationContext validationContext)
        {            
            RequestMessageLabelProvider.Add(validationContext.ObjectInstance);

            try
            {
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);
                if (isValid)
                {
                    return ValidationErrorTree.NoErrors;
                }
                return new ValidationErrorTree(CreateErrorMessagesPerMember(validationResults));                
            }
            finally
            {
                RequestMessageLabelProvider.Remove(validationContext.ObjectInstance);
            }
        }        

        private static Dictionary<string, IList<string>> CreateErrorMessagesPerMember(IEnumerable<ValidationResult> validationResults)
        {
            var errorMessagesPerMember = new Dictionary<string, IList<string>>();

            foreach (var result in validationResults)
            {
                AppendErrorMessage(errorMessagesPerMember, result);
            }
            return errorMessagesPerMember;
        }        

        private static void AppendErrorMessage(IDictionary<string, IList<string>> errorMessageBuilder, ValidationResult result)
        {
            foreach (var memberName in result.MemberNames)
            {
                IList<string> errorMessages;

                if (!errorMessageBuilder.TryGetValue(memberName, out errorMessages))
                {
                    errorMessageBuilder.Add(memberName, errorMessages = new List<string>());
                }
                errorMessages.Add(result.ErrorMessage);
            }
        }
    }
}
