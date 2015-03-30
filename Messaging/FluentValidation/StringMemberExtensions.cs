using System.ComponentModel.Resources;
using System.Text.RegularExpressions;

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

        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that the value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType)
        {
            return IsNotEqualTo(member, other, compareType, new ErrorMessage(ValidationMessages.Member_IsNotEqualTo_Failed, member, other));
        }

        /// <summary>
        /// Verifies that the value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessage)
        {
            return IsNotEqualTo(member, other, compareType, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessageFormat, object arg0)
        {
            return IsNotEqualTo(member, other, compareType, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNotEqualTo(member, other, compareType, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessageFormat, params object[] arguments)
        {
            return IsNotEqualTo(member, other, compareType, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(value => IsNotEqualTo(value, other, compareType), errorMessage);
        }

        private static bool IsNotEqualTo(string value, string other, StringComparison compareType)
        {
            return !string.Equals(value, other, compareType);
        }

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that the value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType)
        {
            return IsEqualTo(member, other, compareType, new ErrorMessage(ValidationMessages.Member_IsEqualTo_Failed, member, other));
        }

        /// <summary>
        /// Verifies that the value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessage)
        {
            return IsEqualTo(member, other, compareType, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessageFormat, object arg0)
        {
            return IsEqualTo(member, other, compareType, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessageFormat, object arg0, object arg1)
        {
            return IsEqualTo(member, other, compareType, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, string errorMessageFormat, params object[] arguments)
        {
            return IsEqualTo(member, other, compareType, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.Satisfies(value => IsEqualTo(value, other, compareType), errorMessage);
        }

        private static bool IsEqualTo(string value, string other, StringComparison compareType)
        {
            return string.Equals(value, other, compareType);
        }

        #endregion

        #region [====== IsNoMatch ======]

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern)
        {
            return IsNoMatch(member, pattern, RegexOptions.None, new ErrorMessage(ValidationMessages.Member_IsNoMatch_Failed, member, pattern));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, string errorMessage)
        {
            return IsNoMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, string errorMessageFormat, object arg0)
        {
            return IsNoMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNoMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, string errorMessageFormat, params object[] arguments)
        {
            return IsNoMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessageFormat, arguments));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, RegexOptions options)
        {
            return IsNoMatch(member, pattern, options, new ErrorMessage(ValidationMessages.Member_IsNoMatch_Failed, member, pattern));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessage)
        {
            return IsNoMatch(member, pattern, options, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessageFormat, object arg0)
        {
            return IsNoMatch(member, pattern, options, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessageFormat, object arg0, object arg1)
        {
            return IsNoMatch(member, pattern, options, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessageFormat, params object[] arguments)
        {
            return IsNoMatch(member, pattern, options, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<string> IsNoMatch(Member<string> member, string pattern, RegexOptions options, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return member.Satisfies(value => IsNoMatch(value, pattern, options), errorMessage);
        }

        private static bool IsNoMatch(string value, string pattern, RegexOptions options)
        {
            return !Regex.IsMatch(value, pattern, options);
        }

        #endregion

        #region [====== IsMatch ======]

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern)
        {
            return IsMatch(member, pattern, RegexOptions.None, new ErrorMessage(ValidationMessages.Member_IsMatch_Failed, member, pattern));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, string errorMessage)
        {
            return IsMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, string errorMessageFormat, object arg0)
        {
            return IsMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, string errorMessageFormat, object arg0, object arg1)
        {
            return IsMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, string errorMessageFormat, params object[] arguments)
        {
            return IsMatch(member, pattern, RegexOptions.None, new ErrorMessage(errorMessageFormat, arguments));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, RegexOptions options)
        {
            return IsMatch(member, pattern, options, new ErrorMessage(ValidationMessages.Member_IsMatch_Failed, member, pattern));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessage)
        {
            return IsMatch(member, pattern, options, new ErrorMessage(errorMessage));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessageFormat, object arg0)
        {
            return IsMatch(member, pattern, options, new ErrorMessage(errorMessageFormat, arg0));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arg0">The first argument of <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessageFormat, object arg0, object arg1)
        {
            return IsMatch(member, pattern, options, new ErrorMessage(errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Verifies that the value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessageFormat">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <param name="arguments">The arguments of <paramref name="errorMessageFormat"/>.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, RegexOptions options, string errorMessageFormat, params object[] arguments)
        {
            return IsMatch(member, pattern, options, new ErrorMessage(errorMessageFormat, arguments));
        }

        private static Member<string> IsMatch(Member<string> member, string pattern, RegexOptions options, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            return member.Satisfies(value => IsMatch(value, pattern, options), errorMessage);
        }

        private static bool IsMatch(string value, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(value, pattern, options);
        }

        #endregion
    }
}
