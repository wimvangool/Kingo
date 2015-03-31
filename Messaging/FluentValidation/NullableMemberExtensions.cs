using System.ComponentModel.Resources;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="Member{TValue}" />.
    /// </summary>
    public static class NullableMemberExtensions
    {        
        #region [====== HasValue ======]

        /// <summary>
        /// Verifies whether or not the <paramref name="member"/>'s value is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param>        
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasValue<TValue>(this Member<TValue?> member) where TValue : struct
        {
            return HasValue(member, new ErrorMessage(ValidationMessages.Member_HasValue_Failed, member));
        }

        /// <summary>
        /// Verifies whether or not the <paramref name="member"/>'s value is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasValue<TValue>(this Member<TValue?> member, ErrorMessage errorMessage) where TValue : struct
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(value => value.HasValue, value => value.Value, null, errorMessage);
        }        

        #endregion
    }
}
