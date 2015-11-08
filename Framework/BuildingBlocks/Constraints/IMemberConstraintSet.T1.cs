using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// When implemented by a class, a <see cref="IMemberConstraintSet{T}" /> can be used to validate the values of
    /// certain members of another instance.
    /// </summary>
    /// <typeparam name="T">Type of the message the constraints apply to.</typeparam>
    public interface IMemberConstraintSet<T>
    {
        #region [====== VerifyThatInstance ======]

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the message itself instead on one of its particular members.
        /// </summary>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/>.</returns>
        IMemberConstraint<T, T> VerifyThatInstance();

        #endregion

        #region [====== VerifyThat ======]

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="fieldOrProperty">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrProperty"/> is not a supported expression.
        /// </exception>
        IMemberConstraint<T, TValue> VerifyThat<TValue>(Expression<Func<T, TValue>> fieldOrProperty);

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyName"/> is not a valid identifier.
        /// </exception>
        IMemberConstraint<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, string fieldOrPropertyName); 

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, Identifier fieldOrPropertyName);        

        #endregion          
        
        #region [====== VerifyThatCollection (IEnumerable<>) ======]

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the items of the collection.</typeparam>
        /// <param name="fieldOrProperty">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrProperty"/> is not a supported expression.
        /// </exception>
        IMemberConstraint<T, IEnumerable<TValue>> VerifyThatCollection<TValue>(Expression<Func<T, IEnumerable<TValue>>> fieldOrProperty);

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the items of the collection.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyName"/> is not a valid identifier.
        /// </exception>
        IMemberConstraint<T, IEnumerable<TValue>> VerifyThatCollection<TValue>(Func<T, IEnumerable<TValue>> fieldOrProperty, string fieldOrPropertyName);

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the items of the collection.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<T, IEnumerable<TValue>> VerifyThatCollection<TValue>(Func<T, IEnumerable<TValue>> fieldOrProperty, Identifier fieldOrPropertyName);

        #endregion

        #region [====== VerifyThatCollection (IReadOnlyDictionary<,>) ======]

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the collection.</typeparam>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="fieldOrProperty">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrProperty"/> is not a supported expression.
        /// </exception>
        IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> VerifyThatCollection<TKey, TValue>(Expression<Func<T, IReadOnlyDictionary<TKey, TValue>>> fieldOrProperty);

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the collection.</typeparam>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyName"/> is not a valid identifier.
        /// </exception>
        IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> VerifyThatCollection<TKey, TValue>(Func<T, IReadOnlyDictionary<TKey, TValue>> fieldOrProperty, string fieldOrPropertyName);

        /// <summary>
        /// Creates and returns a new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys of the collection.</typeparam>
        /// <typeparam name="TValue">Type of the values of the collection.</typeparam>
        /// <param name="fieldOrProperty">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="fieldOrPropertyName">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="IMemberConstraint{T, S}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName"/> is <c>null</c>.
        /// </exception> 
        IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> VerifyThatCollection<TKey, TValue>(Func<T, IReadOnlyDictionary<TKey, TValue>> fieldOrProperty, Identifier fieldOrPropertyName);

        #endregion
    }
}
