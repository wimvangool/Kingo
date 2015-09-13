using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static class BooleanConstraints
    {
        #region [====== IsTrue ======]

        /// <summary>
        /// Verifies that the member's value is <c>true</c>.
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
        public static IMemberConstraint<TMessage, bool> IsTrue<TMessage>(this IMemberConstraint<TMessage, bool> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsTrueConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is true.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, bool, bool> IsTrueConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, bool>(member => member)                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.BooleanConstraints_IsTrue)
                .BuildConstraint();
        }

        #endregion

        #region [====== IsFalse ======]

        /// <summary>
        /// Verifies that the member's value is <c>false</c>.
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
        public static IMemberConstraint<TMessage, bool> IsFalse<TMessage>(this IMemberConstraint<TMessage, bool> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsFalseConstraint<TMessage>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" /> that checks whether or not a value is false.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{TMessage, T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TMessage, bool, bool> IsFalseConstraint<TMessage>(string errorMessage = null)
        {
            return New.Constraint<TMessage, bool>(member => !member)                
                .WithErrorMessage(errorMessage ?? ConstraintErrors.BooleanConstraints_IsFalse)
                .BuildConstraint();
        }

        #endregion
    }
}
