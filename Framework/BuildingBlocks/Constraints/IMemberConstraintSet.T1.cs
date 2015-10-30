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
        /// Creates and returns a new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on the message itself instead on one of its particular members.
        /// </summary>
        /// <returns>A new <see cref="IMemberConstraint{TMessage, S}"/>.</returns>
        IMemberConstraint<TMessage, TMessage> VerifyThatInstance();

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="fieldOrPropertyExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrPropertyExpression"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyExpression"/> is not a supported expression.
        /// </exception>
        IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Expression<Func<TMessage, TValue>> fieldOrPropertyExpression);

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyName"/> is not a valid identifier.
        /// </exception>
        IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> fieldOrProperty, string fieldOrPropertyName); 

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{TMessage, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> fieldOrProperty, Identifier fieldOrPropertyName);                

        #endregion                   
    }
}
