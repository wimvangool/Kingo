using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator{T}" /> that validates a message through
    /// all <see cref="ValidationAttribute">ValidationAttributes</see> that have been declared on the
    /// members of a message.
    /// </summary>    
    public sealed class MessageValidator<TMessage> : IMessageValidator<TMessage> where TMessage : class
    {        
        private readonly Func<TMessage, ValidationContext> _validationContextFactory;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidator{T}" /> class.
        /// </summary>                
        /// <param name="validationContextFactory">
        /// The method used to create a <see cref="ValidationContext" /> for a specific message.
        /// </param>             
        public MessageValidator(Func<TMessage, ValidationContext> validationContextFactory = null)
        {                               
            _validationContextFactory = validationContextFactory ?? (message => new ValidationContext(message, null, null));
        }

        /// <inheritdoc />
        public DataErrorInfo Validate(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Validate(_validationContextFactory.Invoke(message));
        }

        private static DataErrorInfo Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);
            if (isValid)
            {
                return DataErrorInfo.NoErrors;
            }
            return new DataErrorInfo(CreateErrorMessagesPerMember(validationResults));
        }        

        private static Dictionary<string, string> CreateErrorMessagesPerMember(IEnumerable<ValidationResult> validationResults)
        {
            var errorMessagesPerMember = new Dictionary<string, string>();

            foreach (var result in validationResults)
            {
                AppendErrorMessage(errorMessagesPerMember, result);
            }
            return errorMessagesPerMember;
        }        

        private static void AppendErrorMessage(IDictionary<string, string> errorMessageBuilder, ValidationResult result)
        {
            foreach (var memberName in result.MemberNames)
            {
                string errorMessage;

                if (errorMessageBuilder.TryGetValue(memberName, out errorMessage))
                {
                    errorMessageBuilder[memberName] = errorMessage + Environment.NewLine + result.ErrorMessage;
                }
                else
                {
                    errorMessageBuilder.Add(memberName, result.ErrorMessage);
                }
            }
        }
    }
}
