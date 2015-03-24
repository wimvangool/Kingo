namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="Member{String}" />.
    /// </summary>
    public static class StringMemberExtensions
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member, string errorMessage)
        {
            return IsNotNullOrEmpty(member, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>               
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member, string errorMessageFormat, object arg0)        
        {
            return IsNotNullOrEmpty(member, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotNullOrEmpty(member, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>               
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member, string errorMessageFormat, params object[] arguments)
        {
            return IsNotNullOrEmpty(member, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<string> IsNotNullOrEmpty(Member<string> member, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotNullOrEmpty, errorMessage);
        }

        private static bool IsNotNullOrEmpty(string member)
        {
            return !string.IsNullOrEmpty(member);
        }

        #endregion

        #region [====== IsNotNullOrWhiteSpace ======]

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member, string errorMessage)
        {
            return IsNotNullOrWhiteSpace(member, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>               
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member, string errorMessageFormat, object arg0)
        {
            return IsNotNullOrWhiteSpace(member, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotNullOrWhiteSpace(member, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the specified <paramref name="member"/> is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>               
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member, string errorMessageFormat, params object[] arguments)
        {
            return IsNotNullOrWhiteSpace(member, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<string> IsNotNullOrWhiteSpace(Member<string> member, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(IsNotNullOrWhiteSpace, errorMessage);
        }

        private static bool IsNotNullOrWhiteSpace(string member)
        {
            return !string.IsNullOrWhiteSpace(member);
        }

        #endregion        
    }
}
