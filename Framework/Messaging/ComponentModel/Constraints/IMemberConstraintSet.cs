using System;
using System.Linq.Expressions;

namespace ServiceComponents.ComponentModel.Constraints
{
    /// <summary>
    /// When implemented by a class, a <see cref="IMemberConstraintSet" /> can be used to validate the values of
    /// certain members of another instance.
    /// </summary>
    public interface IMemberConstraintSet
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
        /// <returns>A new <see cref="MemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="memberExpression"/> refers to a member that was already added.
        /// </exception>
        IMemberConstraint<TValue> VerifyThat<TValue>(Expression<Func<TValue>> memberExpression);       

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberValueFactory">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="memberName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="MemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberValueFactory"/> or <paramref name="memberName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<TValue> VerifyThat<TValue>(Func<TValue> memberValueFactory, string memberName);        

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberValue">The value to add constraints for.</param>
        /// <param name="memberName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="MemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<TValue> VerifyThat<TValue>(TValue memberValue, string memberName);

        #endregion        
    }
}
