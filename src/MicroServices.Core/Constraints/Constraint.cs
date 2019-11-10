using System;
using System.Collections.Generic;

namespace Kingo.Constraints
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
        public virtual IConstraint Or(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }            
            if (constraint == this)
            {
                return constraint;
            }
            return new OrConstraint(this, constraint);
        }

        #endregion

        #region [====== IsValid ======]                

        /// <inheritdoc />
        public abstract bool IsValid(object value);

        #endregion

        #region [====== Constraint Factory Methods ======]

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
        public static IConstraint Any(params IConstraint[] constraints) =>
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
        public static IConstraint Any(IEnumerable<IConstraint> constraints) =>
            new OrConstraint(constraints);

        /// <summary>
        /// Creates and returns a new constraint that declares any value as valid (with the exception
        /// of <c>null</c>-values and instances that are not an instance of <typeparamref name="TValue"/>).
        /// </summary>
        /// <typeparam name="TValue">Type of value to check.</typeparam>
        /// <returns>
        /// A constraint that is satisfied with any compatible value.
        /// </returns>
        public static IConstraint IsAlwaysValid<TValue>() =>
            FromDelegate<TValue>(value => true);

        /// <summary>
        /// Creates and returns a new constraint that declares any value as invalid.
        /// </summary>
        /// <typeparam name="TValue">Type of value to check.</typeparam>
        /// <returns>
        /// A constraint that is always returns a validation error.
        /// </returns>
        public static IConstraint IsNeverValid<TValue>() =>
            FromDelegate<TValue>(value => false);

        /// <summary>
        /// Creates and returns a new constraint based on the specified delegate <paramref name="constraint"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of value to check.</typeparam>
        /// <param name="constraint">Delegate that represents the implementation of the returned constraint.</param>
        /// <returns>A new constraint based on the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IConstraint FromDelegate<TValue>(Func<TValue, bool> constraint) =>
            new DelegateConstraint<TValue>(constraint);

        #endregion
    }
}
