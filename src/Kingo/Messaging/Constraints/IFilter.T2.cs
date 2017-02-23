using System;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a filter-constraint that transforms an input value to an output value.
    /// </summary>
    /// <typeparam name="TValueIn">Type in the input value.</typeparam>
    /// <typeparam name="TValueOut">Type of the output value.</typeparam>
    public interface IFilter<TValueIn, TValueOut> : IConstraint<TValueIn>
    {
        /// <summary>
        /// Creates and returns a logical AND constraint for this and the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">Another constraint.</param>
        /// <returns>A logical AND constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is <c>null</c>.
        /// </exception>
        IFilter<TValueIn, TResult> And<TResult>(IFilter<TValueOut, TResult> filter);  

        /// <summary>
        /// Determines whether or not the specified <paramref name="valueIn"/> satisfies this constraint.
        /// </summary>
        /// <param name="valueIn">The value to check.</param>
        /// <param name="valueOut">
        /// If this method returns <c>true</c>, will be assigned the output value of this constraint;
        /// otherwise it will be assigned the default value.
        /// </param>
        /// <returns><c>true</c> if the value satisfies this constraint; otherwise <c>false</c>.</returns>
        bool IsSatisfiedBy(TValueIn valueIn, out TValueOut valueOut);

        /// <summary>
        /// Determines whether or not the specified <paramref name="valueIn"/> satisfies this constraint.
        /// </summary>
        /// <param name="valueIn">The value to check.</param>
        /// <param name="errorMessage">
        /// If this method returns <c>true</c>, this parameter will be set to the error of the constraint that failed;
        /// otherwise, it will be <c>null</c>.
        /// </param>
        /// <param name="valueOut">
        /// If this method returns <c>false</c>, will be assigned the output value of this constraint;
        /// otherwise it will be assigned the default value.
        /// </param>
        /// <returns><c>true</c> if the value satisfies this constraint; otherwise <c>false</c>.</returns>
        bool IsNotSatisfiedBy(TValueIn valueIn, out IErrorMessageBuilder errorMessage, out TValueOut valueOut);
    }
}
