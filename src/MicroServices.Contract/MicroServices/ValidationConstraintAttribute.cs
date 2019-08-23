using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a validation-attribute that is implemented through a set of <see cref="IConstraint">constraints</see>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class ValidationConstraintAttribute : ValidationAttribute
    {        
        /// <summary>
        /// When implemented by a class, returns the constraint
        /// that will carry out the validation of (non-null) values.
        /// </summary>
        protected abstract IConstraint Constraint
        {
            get;
        }

        /// <summary>
        /// Creates and returns a new constraint that checks if all specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical AND-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        protected static IConstraint All(params IConstraint[] constraints) =>
            MicroServices.Constraint.All(constraints);

        /// <summary>
        /// Creates and returns a new constraint that checks if all specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical AND-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        protected static IConstraint All(IEnumerable<IConstraint> constraints) =>
            MicroServices.Constraint.All(constraints);

        /// <summary>
        /// Creates and returns a new constraint that checks if any of the specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical OR-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        protected static IOrConstraint Any(params IConstraint[] constraints) =>
            MicroServices.Constraint.Any(constraints);

        /// <summary>
        /// Creates and returns a new constraint that checks if any of the specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical OR-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        protected static IOrConstraint Any(IEnumerable<IConstraint> constraints) =>
            MicroServices.Constraint.Any(constraints);

        /// <summary>
        /// Validates the specified <paramref name="value"/> and returns the validation-result.
        /// This <paramref name="value"/> is <c>null</c> and
        /// if so, whether that value can be accepted. Otherwise, it will invoke the
        /// constraint of this attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">Context of the value that is being validated.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> if <paramref name="value"/> is <c>null</c> or if it is valid;
        /// otherwise it will return a result with the appropriate error message.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
            value == null ? ValidationResult.Success : Constraint.IsValid(value, validationContext);        
    }
}
