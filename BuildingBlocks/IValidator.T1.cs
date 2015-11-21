using System;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific instance.
    /// </summary>
    public interface IValidator<T>
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

        /// <summary>
        /// Combines this validator with the specified <paramref name="validator"/> and returns the composite validator.
        /// </summary>
        /// <param name="validator">The validator to append.</param>
        /// <param name="haltOnFirstError">
        /// Indicates whether or not the composite validator should not invoke the specified <paramref name="validator"/>
        /// when the this validator already detected errors on an instance.
        /// </param>
        /// <returns>
        /// A composite validator that first validates an instance with this validator, then validates the same instance
        /// with the specified <paramref name="validator"/> and then combines the results.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is <c>null</c>.
        /// </exception>
        IValidator<T> Append(IValidator<T> validator, bool haltOnFirstError = false);
    }
}
