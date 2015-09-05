using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static class GuidConstraints
    {
        #region [====== IsNotEmpty ======]

        /// <summary>
        /// Verifies that the value is not an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, Guid> IsNotEmpty<T>(this IMemberConstraint<T, Guid> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsNotEmptyConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a guid is not empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        public static IConstraintWithErrorMessage<Guid, Guid> IsNotEmptyConstraint(string errorMessage = null)
        {
            return New.Constraint<Guid>(member => !member.Equals(Guid.Empty), string.Format("{{member.Name}} != {0}", Guid.Empty))               
                .WithErrorFormat(errorMessage ?? ConstraintErrors.GuidConstraints_IsNotEmpty)
                .BuildConstraint();
        }        

        #endregion

        #region [====== IsEmpty ======]

        /// <summary>
        /// Verifies that the value is an empty <see cref="Guid" />.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, Guid> IsEmpty<T>(this IMemberConstraint<T, Guid> member, string errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(IsEmptyConstraint(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a guid is empty.
        /// </summary>        
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        public static IConstraintWithErrorMessage<Guid, Guid> IsEmptyConstraint(string errorMessage = null)
        {
            return New.Constraint<Guid>(member => member.Equals(Guid.Empty), string.Format("{{member.Name}} == {0}", Guid.Empty))                
                .WithErrorFormat(errorMessage ?? ConstraintErrors.GuidConstraints_IsEmpty)
                .BuildConstraint();
        }        

        #endregion
    }
}
