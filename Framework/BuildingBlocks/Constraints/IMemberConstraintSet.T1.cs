using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, a <see cref="IMemberConstraintSet{TMessage}" /> can be used to validate the values of
    /// certain members of another instance.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message the constraints apply to.</typeparam>
    public interface IMemberConstraintSet<TMessage>
    {
        #region [====== VerifyThat ======]

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{TMessage}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="memberExpression"/> refers to a member that was already added.
        /// </exception>
        IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Expression<Func<TMessage, TValue>> memberExpression);       

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{TMessage}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberValueFactory">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="memberName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberValueFactory"/> or <paramref name="memberName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> memberValueFactory, string memberName);                

        #endregion                   
    }
}
