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
    public static class EnumerableConstraints
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
        public static IMemberConstraint<TMessage, IEnumerable<TValue>> IsNotNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, IEnumerable<TValue>> member, string errorMessage = null)
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
        public static IConstraintWithErrorMessage<TMessage, IEnumerable<TValue>, IEnumerable<TValue>> IsNotNullOrEmptyConstraint<TMessage, TValue>(string errorMessage = null)
        {
            return New.Constraint<TMessage, IEnumerable<TValue>>(member => member != null && member.Any())                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.EnumerableConstraints_IsNotNullOrEmpty)
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
        public static IMemberConstraint<TMessage, IEnumerable<TValue>> IsNullOrEmpty<TMessage, TValue>(this IMemberConstraint<TMessage, IEnumerable<TValue>> member, string errorMessage = null)
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
        public static IConstraintWithErrorMessage<TMessage, IEnumerable<TValue>, IEnumerable<TValue>> IsNullOrEmptyConstraint<TMessage, TValue>(string errorMessage = null)
        {
            return New.Constraint<TMessage, IEnumerable<TValue>>(member => member == null || !member.Any())                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.EnumerableConstraints_IsNullOrEmpty)
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
        public static IMemberConstraint<TMessage, TValue> ElementAt<TMessage, TValue>(this IMemberConstraint<TMessage, IEnumerable<TValue>> member, int index, string errorMessage = null)
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
        public static IConstraintWithErrorMessage<TMessage, IEnumerable<TValue>, TValue> ElementAtConstraint<TMessage, TValue>(int index, string errorMessage = null)
        {
            if (index < 0)
            {
                throw New.NegativeIndexException(index);
            }
            DelegateConstraint<TMessage, IEnumerable<TValue>, TValue>.Implementation implementation = delegate(IEnumerable<TValue> value, TMessage message, out TValue element)
            {                
                if (ReferenceEquals(message, null))
                {
                    throw new ArgumentNullException("message");
                }
                try
                {
                    element = value.Skip(index).First();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    element = default(TValue);
                    return false;
                }
            };
            return New.Constraint(implementation)                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.EnumerableConstraints_ElementAt)
                .WithErrorMessageArguments(new { Index = index })
                .BuildConstraint();
        }       

        #endregion
    }
}
