using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.Validation
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IConstraint"/> interface.
    /// </summary>
    public abstract class Constraint : IConstraint
    {
        #region [====== Logical Operations ======]

        /// <inheritdoc />
        public virtual IConstraint And(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            if (constraint == this)
            {
                return constraint;
            }
            return new AndConstraint(this, constraint);
        }

        /// <inheritdoc />
        public virtual IOrConstraint Or(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }            
            return new OrConstraint(this, constraint);
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
        public static IConstraint All(params IConstraint[] constraints) =>
            new AndConstraint(constraints);

        /// <summary>
        /// Creates and returns a new constraint that checks if all specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical AND-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        public static IConstraint All(IEnumerable<IConstraint> constraints) =>
            new AndConstraint(constraints);

        /// <summary>
        /// Creates and returns a new constraint that checks if any of the specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical OR-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        public static IOrConstraint Any(params IConstraint[] constraints) =>
            new OrConstraint(constraints);

        /// <summary>
        /// Creates and returns a new constraint that checks if any of the specified <paramref name="constraints"/>
        /// are satisfied by a specific value.
        /// </summary>
        /// <param name="constraints">The constraints to check.</param>
        /// <returns>A new constraint that represents a logical OR-operation for all specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        public static IOrConstraint Any(IEnumerable<IConstraint> constraints) =>
            new OrConstraint(constraints);

        #endregion

        #region [====== Validation ======]                

        /// <inheritdoc />
        public abstract ValidationResult IsValid(object value, ValidationContext validationContext);

        /// <summary>
        /// Returns a new constraint that declares any value as valid (with the exception
        /// of <c>null</c>-values and instances that are not an instance of <typeparamref name="TValue"/>).
        /// </summary>
        /// <typeparam name="TValue">Type of value to check.</typeparam>
        /// <returns>
        /// A constraint that is satisfied with any compatible value.
        /// </returns>
        public static IConstraint IsAlwaysValid<TValue>() =>
            new DelegateConstraint<TValue>((value, context) => ValidationResult.Success);

        /// <summary>
        /// Returns a new constraint that declares any value as invalid.
        /// </summary>
        /// <typeparam name="TValue">Type of value to check.</typeparam>
        /// <param name="errorMessageFormat">
        /// The error message that will be returned in formatted form when the constraint's
        /// <see cref="IConstraint.IsValid(object, ValidationContext)"/> method is called.
        /// If not specified, a default error message is used.
        /// </param>
        /// <returns>
        /// A constraint that is always returns a validation error.
        /// </returns>
        public static IConstraint IsNeverValid<TValue>(string errorMessageFormat = null) =>
            new DelegateConstraint<TValue>((value, context) => NewErrorResult(errorMessageFormat, value));

        private static ValidationResult NewErrorResult(string errorMessageFormat, object value)
        {
            var messageFormat = errorMessageFormat ?? ErrorMessages.Constraint_ValueNotValid;
            var message = string.Format(messageFormat, value);
            return new ValidationResult(message);
        }

        #endregion
    }
}
