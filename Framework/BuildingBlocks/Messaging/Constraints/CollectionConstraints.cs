using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static class CollectionConstraints
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection has at least one element.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, ICollection<TValue>> IsNotNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotNullOrEmptyConstraint<TMessage, TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a collection is not null or empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, ICollection<TValue>, ICollection<TValue>> IsNotNullOrEmptyConstraint<TMessage, TValue>(string errorMessage = null)
        {
            return New.Constraint<TMessage, ICollection<TValue>>(member => member != null && member.Count > 0)                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.CollectionConstraints_IsNotNullOrEmpty)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified collection is either <c>null</c> or empty.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, ICollection<TValue>> IsNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNullOrEmptyConstraint<TMessage, TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a collection is null or empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, ICollection<TValue>, ICollection<TValue>> IsNullOrEmptyConstraint<TMessage, TValue>(string errorMessage = null)
        {
            return New.Constraint<TMessage, ICollection<TValue>>(member => member == null || member.Count == 0)                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.CollectionConstraints_IsNullOrEmpty)
                .BuildConstraint();
        }

        #endregion

        #region [====== ElementAt ======]

        /// <summary>
        /// Verifies that the specified collection contains a value at the specified <paramref name="index"/>.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="index">The index to verify.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, ICollection<TValue>> member, int index, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }                                   
            Func<string, string> nameSelector = name => string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", name, index);

            return member.Satisfies(ElementAtConstraint<TMessage, TValue>(index, errorMessage), nameSelector);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a collection has an element at the specified index.
        /// </summary>        
        /// <param name="index">The index to verify.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, ICollection<TValue>, TValue> ElementAtConstraint<TMessage, TValue>(int index, string errorMessage = null)
        {
            if (index < 0)
            {
                throw New.NegativeIndexException(index);
            }
            return New.Constraint<TMessage, ICollection<TValue>, TValue>(member => index < member.Count, member => member.ElementAt(index))                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.CollectionConstraints_ElementAt)
                .WithErrorMessageArguments(new { Index = index })
                .BuildConstraint();
        }

        #endregion
    }
}
