using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a constraint for a certain value.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public interface IConstraint<TValue>
    {
        #region [====== And, Or & Invert ======]

        /// <summary>
        /// Creates and returns a logical AND constraint for this and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <param name="errorMessage">Error message associated with the constraint.</param>
        /// <param name="name">Name of the constraint.</param>
        /// <returns>A logical AND constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraint<TValue> And(Func<TValue, bool> constraint, string errorMessage = null, string name = null);

        /// <summary>
        /// Creates and returns a logical AND constraint for this and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <param name="errorMessage">Error message associated with the constraint.</param>
        /// <param name="name">Name of the constraint.</param>
        /// <returns>A logical AND constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraint<TValue> And(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name = null); 

        /// <summary>
        /// Creates and returns a logical AND constraint for this and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <returns>A logical AND constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraint<TValue> And(IConstraint<TValue> constraint);

        /// <summary>
        /// Creates and returns a logical OR constraint for this and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <param name="errorMessage">Error message associated with the constraint.</param>
        /// <param name="name">Name of the constraint.</param>
        /// <returns>A logical OR constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraintWithErrorMessage<TValue> Or(Func<TValue, bool> constraint, string errorMessage = null, string name = null);

        /// <summary>
        /// Creates and returns a logical OR constraint for this and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <param name="errorMessage">Error message associated with the constraint.</param>
        /// <param name="name">Name of the constraint.</param>
        /// <returns>A logical OR constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraintWithErrorMessage<TValue> Or(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name = null);

        /// <summary>
        /// Creates and returns a logical OR constraint for this and the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">Another constraint.</param>
        /// <returns>A logical OR constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint);

        /// <summary>
        /// Creates and returns a constraint that negates this constraint.
        /// </summary>        
        /// <returns>A constraint that is the logical opposite of this constraint.</returns>
        IConstraint<TValue> Invert();        

        #endregion

        #region [====== MapInputToOutput ======]

        /// <summary>
        /// Converts this constraint to a constraint that maps the input to the output.
        /// </summary>
        /// <returns>A new constraint wrapping the current constraint that maps the input to the output.</returns>
        IConstraint<TValue, TValue> MapInputToOutput();

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> satisfies this constraint.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the value satisfies this constraint; otherwise <c>false</c>.</returns>
        bool IsSatisfiedBy(TValue value);

        /// <summary>
        /// Determines whether or not the specified <paramref name="value"/> satisfies this constraint.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="errorMessage">
        /// If this method returns <c>true</c>, this parameter will be set to the error of the constraint that failed;
        /// otherwise, it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the value satisfies this constraint; otherwise <c>false</c>.</returns>
        bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage);

        #endregion
    }
}
