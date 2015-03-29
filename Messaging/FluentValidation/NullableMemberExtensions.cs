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
        /// Verifies whether or not the specified <paramref name="member" /> is not <c>null</c>.
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
        /// Verifies whether or not the specified <paramref name="member" /> is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasValue<TValue>(this Member<TValue?> member, string errorMessage) where TValue : struct
        {
            return HasValue(member, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies whether or not the specified <paramref name="member" /> is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasValue<TValue>(this Member<TValue?> member, string errorMessageFormat, object arg0) where TValue : struct
        {
            return HasValue(member, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies whether or not the specified <paramref name="member" /> is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>     
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>  
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasValue<TValue>(this Member<TValue?> member, string errorMessageFormat, object arg0, object arg1) where TValue : struct
        {
            return HasValue(member, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies whether or not the specified <paramref name="member" /> is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<TValue> HasValue<TValue>(this Member<TValue?> member, string errorMessageFormat, params object[] arguments) where TValue : struct
        {
            return HasValue(member, new ErrorMessage(errorMessageFormat, arguments));
        }
        
        private static Member<TValue> HasValue<TValue>(Member<TValue?> member, ErrorMessage errorMessage) where TValue : struct
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(value => value.HasValue, errorMessage, value => value.Value);
        }        

        #endregion
    }
}
