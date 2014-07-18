using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace YellowFlare.MessageProcessing.Messages
{
    public abstract class MessageValidationAttribute : ValidationAttribute
    {
        protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                return IsValid(value, null, null);
            }
            var propertyMapping = MessagePropertyLabelCollection.For(validationContext.ObjectInstance);
            var propertyName = propertyMapping.FindLabelFor(validationContext.MemberName);

            return IsValid(value, validationContext, propertyName);
        }

        protected abstract ValidationResult IsValid(object value, ValidationContext validationContext, string propertyName);

        protected static readonly ValidationResult Success = ValidationResult.Success;

        protected static TService Resolve<TService>(ValidationContext validationContext) where TService : class
        {
            return validationContext == null ? null : validationContext.GetService(typeof(TService)) as TService;
        }

        protected static ValidationResult CreateValidationError(string errorMessage, string propertyName)
        {
            return new ValidationResult(errorMessage, new[] { propertyName });
        }
    }
}
