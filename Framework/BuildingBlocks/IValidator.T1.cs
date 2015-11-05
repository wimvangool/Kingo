using System;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific instance.
    /// </summary>
    public interface IValidator<in T>
    {
        /// <summary>
        /// Validates all values of the specified <paramref name="instance"/> and returns all the validation-errors, if any.
        /// </summary>   
        /// <param name="instance">The instance to validate.</param>             
        /// <returns>
        /// A <see cref="ErrorInfo" /> instance that contains all validation errors, if any.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <c>null</c>.
        /// </exception>   
        ErrorInfo Validate(T instance); 
    }
}
