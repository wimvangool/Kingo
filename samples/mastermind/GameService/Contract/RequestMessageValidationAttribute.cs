using System;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MasterMind.GameService
{
    /// <summary>
    /// Serves as a base-class for all custom <see cref="ValidationAttribute">validation-attributes</see>
    /// implemented for this service.
    /// </summary>
    public abstract class RequestMessageValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessageValidationAttribute" /> class.
        /// </summary>
        /// <param name="errorMessageResourceName">
        /// Specifies which error-message in the resource-file must be used to show 
        /// </param>
        protected RequestMessageValidationAttribute(string errorMessageResourceName)
        {
            ErrorMessageResourceType = typeof(ErrorMessages);
            ErrorMessageResourceName = errorMessageResourceName ?? throw new ArgumentNullException(nameof(errorMessageResourceName));
        }
    }
}
