using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint over a certain <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TMessage">Type of a certain message.</typeparam>
    /// <typeparam name="TValue">Type of the value this constraint is for.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public interface IConstraintWithErrorMessage<TMessage, in TValue, TResult> : IConstraint<TMessage, TValue>
    {
        /// <summary>
        /// Merges this constraint with another constraint using a logical AND.
        /// </summary>
        /// <param name="constraint">The constraint to merge with.</param>
        /// <returns>The merged constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IConstraintWithErrorMessage<TMessage, TValue, TNewResult> And<TNewResult>(IConstraintWithErrorMessage<TMessage, TResult, TNewResult> constraint);        

        /// <summary>
        /// Determines whether or not this constraint is satisfied by the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value of the member to check.</param>
        /// <param name="message">Message the member belongs to.</param>
        /// <param name="result">
        /// If this method returns <c>true</c>, refers to the (possibly converted) value that was checked by this constraint;
        /// otherwise, it will be assigned the default value of <typeparamref name="TResult"/>.
        /// </param>
        /// <param name="failedConstraint">
        /// If this method returns <c>false</c>, refers to the constraint that failed; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if this constraint was satisfied by the specified <paramref name="value"/>; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        bool IsSatisfiedBy(TValue value, TMessage message, out TResult result, out IConstraintWithErrorMessage<TMessage> failedConstraint);
    }
}
