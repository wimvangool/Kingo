using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static class CollectionConstraints
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection has at least one element.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<ICollection<TValue>> IsNotNullOrEmpty<TValue>(this IMemberConstraint<ICollection<TValue>> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotNullOrEmptyConstraint<TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a collection is not null or empty.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<ICollection<TValue>, ICollection<TValue>> IsNotNullOrEmptyConstraint<TValue>(string errorMessage = null)
        {
            return New.Constraint<ICollection<TValue>>(collection => collection != null && collection.Count > 0)
                .WithDisplayFormat("{member.Name}?.Count > 0")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.CollectionConstraints_IsNotNullOrEmpty)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection is either <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<ICollection<TValue>> IsNullOrEmpty<TValue>(this IMemberConstraint<ICollection<TValue>> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullOrEmptyConstraint<TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a collection is null or empty.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<ICollection<TValue>, ICollection<TValue>> IsNullOrEmptyConstraint<TValue>(string errorMessage = null)
        {
            return New.Constraint<ICollection<TValue>>(collection => collection == null || collection.Count == 0)
                .WithDisplayFormat("{member.Name} == null || {member.Name}.Count == 0")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.CollectionConstraints_IsNullOrEmpty)
                .BuildConstraint();
        }

        #endregion

        #region [====== ElementAt ======]

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the values in the collection.</typeparam>
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TValue> ElementAt<TValue>(this IMemberConstraint<ICollection<TValue>> member, int index, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                                   
            Func<string, string> nameSelector = name => string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", name, index);

            return member.Satisfies(ElementAtConstraint<TValue>(index, errorMessage), nameSelector);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a collection has an element at the specified index.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="index">The index to verify.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public static IConstraintWithErrorMessage<ICollection<TValue>, TValue> ElementAtConstraint<TValue>(int index, string errorMessage = null)
        {
            if (index < 0)
            {
                throw New.NegativeIndexException(index);
            }
            return New.Constraint<ICollection<TValue>, TValue>(collection => index < collection.Count, collection => collection.ElementAt(index))
                .WithDisplayFormat("{member.Index} < {member.Name}.Count")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.CollectionConstraints_ElementAt)
                .WithArguments(new { Index = index })
                .BuildConstraint();
        }

        #endregion
    }
}
