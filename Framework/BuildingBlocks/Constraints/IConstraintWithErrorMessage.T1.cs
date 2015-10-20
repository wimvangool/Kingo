using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a constraint with a name and associated error message.
    /// </summary>
    public interface IConstraintWithErrorMessage<TValue> : IConstraint<TValue>, IConstraintWithErrorMessage
    {
        #region [====== Name & ErrorMessage ======]

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">New name of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is not valid <see cref="Identifier" />.
        /// </exception>
        new IConstraintWithErrorMessage<TValue> WithName(string name);

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">New name of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>        
        new IConstraintWithErrorMessage<TValue> WithName(Identifier name);

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">New error message of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="errorMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        new IConstraintWithErrorMessage<TValue> WithErrorMessage(string errorMessage);

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">New error message of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="errorMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>        
        new IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <summary>
        /// Creates and returns a constraint that negates this constraint.
        /// </summary>
        /// <param name="errorMessage">Error message of the inverting constraint.</param>
        /// <param name="name">Name of the inverting constraint.</param>
        /// <returns>A constraint that is the logical opposite of this constraint.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format or <paramref name="name"/> is not a valid identifier.
        /// </exception>
        IConstraintWithErrorMessage<TValue> Invert(string errorMessage, string name = null);

        /// <summary>
        /// Creates and returns a constraint that negates this constraint.
        /// </summary>
        /// <param name="errorMessage">Error message of the inverting constraint.</param>
        /// <param name="name">Name of the inverting constraint.</param>
        /// <returns>A constraint that is the logical opposite of this constraint.</returns>
        IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null);

        #endregion
    }
}
