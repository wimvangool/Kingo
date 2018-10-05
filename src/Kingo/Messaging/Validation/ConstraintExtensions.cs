using System;
using System.ComponentModel.DataAnnotations;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains extension methods for <see cref="IConstraint"/> instances.
    /// </summary>
    public static class ConstraintExtensions
    {
        #region [====== Logical Operations ======]

        /// <summary>
        /// Creates and returns a new constraints that checks if a value satisfies
        /// both this constraint and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="thisConstraint">First constraint to check.</param>
        /// <param name="constraint">Second constraint to check.</param>
        /// <returns>
        /// A new constraint that represents the logical AND between this and the specified <paramref name="constraint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IConstraint And<TValue>(this IConstraint thisConstraint, Func<TValue, ValidationContext, ValidationResult> constraint) =>
            thisConstraint.And(new DelegateConstraint<TValue>(constraint));

        /// <summary>
        /// Creates and returns a new constraints that checks if a value satisfies
        /// either this constraint or the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="thisConstraint">First constraint to check.</param>
        /// <param name="constraint">Second constraint to check.</param>
        /// <returns>
        /// A new constraint that represents the logical OR between this and the specified <paramref name="constraint"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IOrConstraint Or<TValue>(this IConstraint thisConstraint, Func<TValue, ValidationContext, ValidationResult> constraint) =>
            thisConstraint.Or(new DelegateConstraint<TValue>(constraint));

        #endregion

        #region [====== Validation ======]   

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns whether or not the value was invalid.
        /// </summary>
        /// <param name="thisConstraint">The constraint to check.</param>
        /// <param name="value">The value to validate.</param>        
        /// <param name="result">
        /// <see cref="ValidationResult.Success"/> if <paramref name="value"/> is valid; otherwise
        /// it will be a result with the appropriate error message.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was not valid; otherwise <c>false</c>.
        /// </returns>
        public static bool IsNotValid(this IConstraint thisConstraint, object value, out ValidationResult result) =>
            thisConstraint.IsNotValid(value, CreateValidationContext(value), out result);

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns whether or not the value was invalid.
        /// </summary>
        /// <param name="thisConstraint">The constraint to check.</param>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">Context of the value that is being validated.</param>
        /// <param name="result">
        /// <see cref="ValidationResult.Success"/> if <paramref name="value"/> is valid; otherwise
        /// it will be a result with the appropriate error message.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was not valid; otherwise <c>false</c>.
        /// </returns>
        public static bool IsNotValid(this IConstraint thisConstraint, object value, ValidationContext validationContext, out ValidationResult result) =>
            (result = thisConstraint.IsValid(value, validationContext)) != ValidationResult.Success;

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns the validation-result.        
        /// </summary>
        /// <param name="thisConstraint">The constraint to check.</param>
        /// <param name="value">The value to validate.</param>        
        /// <returns>
        /// <see cref="ValidationResult.Success"/> if <paramref name="value"/> is valid; otherwise
        /// it will return a result with the appropriate error message.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not an instance of the expected type.
        /// </exception>
        public static ValidationResult IsValid(this IConstraint thisConstraint, object value) =>
            thisConstraint.IsValid(value, CreateValidationContext(value));

        private static ValidationContext CreateValidationContext(object value) =>
            new ValidationContext(value ?? new object());

        #endregion
    }
}
