using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// When implemented by a class, a <see cref="IMemberConstraintSet{T}" /> can be used to validate the values of
    /// certain members of another instance.
    /// </summary>
    /// <typeparam name="T">Type of the object the constraints are added for.</typeparam>
    public interface IMemberConstraintSet<T>
    {
        #region [====== VerifyThat ======]

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="memberExpression"/> refers to a member that was already added.
        /// </exception>
        IMemberConstraint<T, TValue> VerifyThat<TValue>(Expression<Func<T, TValue>> memberExpression);       

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberValueFactory">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="memberName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberValueFactory"/> or <paramref name="memberName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<T, TValue> VerifyThat<TValue>(Func<T, TValue> memberValueFactory, string memberName);                

        #endregion        
    }
}
