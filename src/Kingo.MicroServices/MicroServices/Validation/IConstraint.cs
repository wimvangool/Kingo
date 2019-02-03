using System;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.Validation
{
    /// <summary>
    /// Represents a constraint that can be used to validate a specific value.
    /// </summary>
    public interface IConstraint
    {
        #region [====== Logical Operations ======]        

        /// <summary>
        /// Creates and returns a new constraints that checks if a value satisfies
        /// both this constraint and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <returns>
        /// A new constraint that represents the logical AND between this and the specified <paramref name="constraint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraint And(IConstraint constraint);

        /// <summary>
        /// Creates and returns a new constraints that checks if a value satisfies
        /// either this constraint or the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <returns>
        /// A new constraint that represents the logical OR between this and the specified <paramref name="constraint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IOrConstraint Or(IConstraint constraint);

        #endregion

        #region [====== Validation ======]               

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns the validation-result.        
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">Context of the value that is being validated.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> if <paramref name="value"/> is valid; otherwise
        /// it will return a result with the appropriate error message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="validationContext"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not an instance of the expected type.
        /// </exception>
        ValidationResult IsValid(object value, ValidationContext validationContext);

        #endregion
    }
}
