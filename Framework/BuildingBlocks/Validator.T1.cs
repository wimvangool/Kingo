using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Represents a <see cref="IValidator{T}" /> that validates an instance through
    /// all <see cref="ValidationAttribute">ValidationAttributes</see> that have been declared on the
    /// members of an instance.
    /// </summary>    
    public sealed class Validator<T> : IValidator<T> where T : class
    {        
        private readonly Func<T, ValidationContext> _validationContextFactory;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator{T}" /> class.
        /// </summary>                
        /// <param name="validationContextFactory">
        /// The method used to create a <see cref="ValidationContext" /> for a specific instance.
        /// </param>             
        public Validator(Func<T, ValidationContext> validationContextFactory = null)
        {                               
            _validationContextFactory = validationContextFactory ?? (instance => new ValidationContext(instance, null, null));
        }

        /// <inheritdoc />
        public ErrorInfo Validate(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            var errorInfo = Validate(_validationContextFactory.Invoke(instance));
            if (errorInfo.HasErrors)
            {
                return ErrorInfo.Empty;
            }
            return errorInfo;
        }

        private static ErrorInfo Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);
            if (isValid)
            {
                return ErrorInfo.Empty;
            }
            return new ErrorInfo(CreateErrorMessagesPerMember(validationResults));
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
