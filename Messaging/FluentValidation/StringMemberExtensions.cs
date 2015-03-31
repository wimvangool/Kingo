﻿using System.ComponentModel.Resources;
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
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>        
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member)
        {
            return IsNotNullOrEmpty(member, new ErrorMessage(ValidationMessages.Member_IsNotNullOrEmpty_Failed, member));
        }        

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member, ErrorMessage errorMessage)
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
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>        
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member)
        {
            return IsNotNullOrWhiteSpace(member, new ErrorMessage(ValidationMessages.Member_IsNotNullOrWhiteSpace_Failed, member));
        }        

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or consists only of white space.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member, ErrorMessage errorMessage)
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
        /// Verifies that the <paramref name="member" />'s value is not equal to <paramref name="other"/>.
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
        /// Verifies that the <paramref name="member" />'s value is not equal to <paramref name="other"/>.
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
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, ErrorMessage errorMessage)
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
        /// Verifies that the <paramref name="member" />'s value is equal to <paramref name="other"/>.
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
        /// Verifies that the <paramref name="member" />'s value is equal to <paramref name="other"/>.
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
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, ErrorMessage errorMessage)
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

        #region [====== StartsWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> StartsWith(this Member<string> member, string prefix)
        {
            return StartsWith(member, prefix, StringComparison.Ordinal, new ErrorMessage(ValidationMessages.Member_StartsWith_Failed, member, prefix));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="prefix"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> StartsWith(this Member<string> member, string prefix, ErrorMessage errorMessage)
        {
            return StartsWith(member, prefix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> StartsWith(this Member<string> member, string prefix, StringComparison compareType)
        {
            return StartsWith(member, prefix, compareType, new ErrorMessage(ValidationMessages.Member_StartsWith_Failed, member, prefix));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="prefix"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> StartsWith(this Member<string> member, string prefix, StringComparison compareType, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            return member.Satisfies(value => StartsWith(value, prefix, compareType), errorMessage);
        }

        private static bool StartsWith(string value, string prefix, StringComparison compareType)
        {
            return value != null && value.StartsWith(prefix, compareType);
        }

        #endregion

        #region [====== EndsWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> EndsWith(this Member<string> member, string postfix)
        {
            return EndsWith(member, postfix, StringComparison.Ordinal, new ErrorMessage(ValidationMessages.Member_EndsWith_Failed, member, postfix));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="postfix"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> EndsWith(this Member<string> member, string postfix, ErrorMessage errorMessage)
        {
            return EndsWith(member, postfix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> EndsWith(this Member<string> member, string postfix, StringComparison compareType)
        {
            return EndsWith(member, postfix, compareType, new ErrorMessage(ValidationMessages.Member_EndsWith_Failed, member, postfix));
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="postfix"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> EndsWith(this Member<string> member, string postfix, StringComparison compareType, ErrorMessage errorMessage)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            return member.Satisfies(value => EndsWith(value, postfix, compareType), errorMessage);
        }

        private static bool EndsWith(string value, string postfix, StringComparison compareType)
        {
            return value != null && value.EndsWith(postfix, compareType);
        }

        #endregion

        #region [====== IsNoMatch ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
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
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="pattern"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, ErrorMessage errorMessage)
        {
            return IsNoMatch(member, pattern, RegexOptions.None, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
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
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="pattern"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNoMatch(this Member<string> member, string pattern, RegexOptions options, ErrorMessage errorMessage)
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
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
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
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="pattern"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, ErrorMessage errorMessage)
        {
            return IsMatch(member, pattern, RegexOptions.None, errorMessage);
        }        

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
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
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="pattern"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsMatch(this Member<string> member, string pattern, RegexOptions options, ErrorMessage errorMessage)
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
