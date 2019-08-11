using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Contains extension-methods for every object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether or not the specified <paramref name="instance"/> is valid, and provides all validation errors
        /// if it's not.
        /// </summary>
        /// <param name="instance">The message to validate.</param>
        /// <param name="results">
        /// If this instance is not valid, this collection will contain all validation-errors; will be <c>null</c> otherwise.
        /// </param>
        /// <param name="serviceProvider">
        /// Optional service provider that can be used to obtain services for validating this instance.
        /// </param>        
        /// <returns><c>true</c> if the instance is not valid; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <c>null</c>.
        /// </exception>
        public static bool IsNotValid(this object instance, out ICollection<ValidationResult> results, IServiceProvider serviceProvider = null) =>
            IsNotValid(new ValidationContext(instance, serviceProvider, null), out results);

        internal static bool IsNotValid(ValidationContext validationContext, out ICollection<ValidationResult> results)
        {
            if (Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, results = new List<ValidationResult>(), true))
            {
                results = null;
                return false;
            }
            return true;
        }
    }
}
