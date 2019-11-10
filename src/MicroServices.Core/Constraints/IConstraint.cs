using System;

namespace Kingo.Constraints
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
        IConstraint Or(IConstraint constraint);

        #endregion

        #region [====== Validation ======]               

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns the validation-result.        
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> satisfies this constraint; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not an instance of the expected type.
        /// </exception>
        bool IsValid(object value);

        #endregion
    }
}
