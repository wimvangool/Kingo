using System;

namespace Kingo.Constraints
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
        public static IConstraint And<TValue>(this IConstraint thisConstraint, Func<TValue, bool> constraint) =>
            thisConstraint.And(Constraint.FromDelegate(constraint));

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
        public static IConstraint Or<TValue>(this IConstraint thisConstraint, Func<TValue, bool> constraint) =>
            thisConstraint.Or(Constraint.FromDelegate(constraint));

        #endregion
    }
}
