using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsNotNull ======]

        /// <summary>
        /// Verifies whether or not the member's value is not <c>null</c>.
        /// </summary>     
        /// <param name="member">A member.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotNull<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== IsNull ======]

        /// <summary>
        /// Verifies whether or not the member's value is <c>null</c>.
        /// </summary> 
        /// <param name="member">A member.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNull<TMessage, TValue>(this IMemberConstraint<TMessage, TValue> member, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion        
    }
}
