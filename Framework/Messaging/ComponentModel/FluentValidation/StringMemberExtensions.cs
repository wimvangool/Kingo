using System.Globalization;
using System.Resources;
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrEmpty(this Member<string> member, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsNotNullOrEmpty_Failed, member);
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotNullOrWhiteSpace(this Member<string> member, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }     
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsNotNullOrWhiteSpace_Failed, member);
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsNotEqualTo(this Member<string> member, string other, StringComparison compareType, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.Member_IsNotEqualTo_Failed, member, other);
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> IsEqualTo(this Member<string> member, string other, StringComparison compareType, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.Member_IsEqualTo_Failed, member, other);
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> StartsWith(this Member<string> member, string prefix, ErrorMessage errorMessage = null)
        {
            return StartsWith(member, prefix, StringComparison.Ordinal, errorMessage);
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
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> StartsWith(this Member<string> member, string prefix, StringComparison compareType, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_StartsWith_Failed, member, prefix);
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
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> EndsWith(this Member<string> member, string postfix, ErrorMessage errorMessage = null)
        {
            return EndsWith(member, postfix, StringComparison.Ordinal, errorMessage);
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
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        public static Member<string> EndsWith(this Member<string> member, string postfix, StringComparison compareType, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_EndsWith_Failed, member, postfix);
            }
            return member.Satisfies(value => EndsWith(value, postfix, compareType), errorMessage);
        }

        private static bool EndsWith(string value, string postfix, StringComparison compareType)
        {
            return value != null && value.EndsWith(postfix, compareType);
        }

        #endregion

        #region [====== DoesNotContain ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> DoesNotContain(this Member<string> member, char value, ErrorMessage errorMessage = null)
        {
            return DoesNotContain(member, value.ToString(CultureInfo.CurrentCulture), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public static Member<string> DoesNotContain(this Member<string> member, string value, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_DoesNotContain_Failed, value);
            }
            return member.Satisfies(memberValue => DoesNotContain(memberValue, value), errorMessage);
        }

        private static bool DoesNotContain(string value, string other)
        {
            return !value.Contains(other);
        }

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<string> Contains(this Member<string> member, char value, ErrorMessage errorMessage = null)
        {
            return Contains(member, value.ToString(CultureInfo.CurrentCulture), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public static Member<string> Contains(this Member<string> member, string value, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_Contains_Failed, value);
            }
            return member.Satisfies(memberValue => Contains(memberValue, value), errorMessage);
        }

        private static bool Contains(string value, string other)
        {
            return value.Contains(other);
        }

        #endregion

        #region [====== DoesNotMatch ======]

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
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> DoesNotMatch(this Member<string> member, string pattern, ErrorMessage errorMessage = null)
        {
            return DoesNotMatch(member, pattern, RegexOptions.None, errorMessage);
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
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> DoesNotMatch(this Member<string> member, string pattern, RegexOptions options, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_DoesNotMatch_Failed, member, pattern);
            }
            return member.Satisfies(value => DoesNotMatch(value, pattern, options), errorMessage);
        }

        private static bool DoesNotMatch(string value, string pattern, RegexOptions options)
        {
            return !Regex.IsMatch(value, pattern, options);
        }

        #endregion

        #region [====== Matches ======]

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
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> Matches(this Member<string> member, string pattern, ErrorMessage errorMessage = null)
        {
            return Matches(member, pattern, RegexOptions.None, errorMessage);
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
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public static Member<string> Matches(this Member<string> member, string pattern, RegexOptions options, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_Matches_Failed, member, pattern);
            }
            return member.Satisfies(value => Matches(value, pattern, options), errorMessage);
        }

        private static bool Matches(string value, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(value, pattern, options);
        }

        #endregion        

        #region [====== Length ======]
      
        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is equal to <paramref name="length"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="length">The required length of the string.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is smaller than <c>0</c>.
        /// </exception>
        public static Member<string> HasLengthOf(this Member<string> member, int length, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (length < 0)
            {
                throw NewNegativeLengthException("length", length);
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.Member_HasLengthOf_Failed, length);
            }
            return member.HasLengthBetween(new Range<int>(length, length), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is between <paramref name="minimum"/> and <paramref name="maximum"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="minimum">The minimum length of the string (inclusive).</param>
        /// <param name="maximum">The maximum length of the string (inclusive).</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maximum"/> is smaller than <paramref name="minimum"/> or smaller than <c>0</c>.
        /// </exception>
        public static Member<string> HasLengthBetween(this Member<string> member, int minimum, int maximum, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (maximum < 0)
            {
                throw NewNegativeLengthException("maximum", maximum);
            }
            return HasLengthBetween(member, new Range<int>(minimum, maximum), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is in the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="range">A range of allowable lengths.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="range" /> is <c>null</c>.
        /// </exception>        
        public static Member<string> HasLengthBetween(this Member<string> member, IRange<int> range, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }            
            return member.Satisfies(value => HasLengthBetween(value, range), errorMessage);
        }

        private static bool HasLengthBetween(string value, IRange<int> range)
        {
            return value != null && range.Contains(value.Length);
        }

        private static Exception NewNegativeLengthException(string paramName, int length)
        {
            var messageFormat = ExceptionMessages.StringMemberExtensions_NegativeLength;
            var message = string.Format(messageFormat, length);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion

        #region [====== IsByte ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="byte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<byte> IsByte(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsByte(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }        

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="byte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<byte> IsByte(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsByte_Failed, member);
            }
            return IsNumber<byte>(member, style, provider, errorMessage, byte.TryParse);
        }

        #endregion

        #region [====== IsSByte ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<sbyte> IsSByte(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsSByte(member, NumberStyles.Integer | NumberStyles.AllowTrailingSign, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<sbyte> IsSByte(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsSByte_Failed, member);
            }
            return IsNumber<sbyte>(member, style, provider, errorMessage, sbyte.TryParse);            
        }

        #endregion

        #region [====== IsChar ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="char"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The only character of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<char> IsChar(this Member<string> member, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsChar_Failed, member);
            }
            Func<string, bool> constraint = value => value != null && value.Length == 1;
            Func<string, char> selector = value => value[0];

            return member.Satisfies(constraint, selector, null, errorMessage);
        }

        #endregion

        #region [====== IsInt16 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="short"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="short"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<short> IsInt16(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsInt16(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="short"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="short"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<short> IsInt16(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsInt16_Failed, member);
            }
            return IsNumber<short>(member, style, provider, errorMessage, short.TryParse);
        }

        #endregion

        #region [====== IsUInt16 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="ushort"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="ushort"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<ushort> IsUInt16(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsUInt16(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="ushort"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="ushort"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<ushort> IsUInt16(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsUInt16_Failed, member);
            }
            return IsNumber<ushort>(member, style, provider, errorMessage, ushort.TryParse);
        }

        #endregion

        #region [====== IsInt32 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="int"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<int> IsInt32(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsInt32(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="int"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<int> IsInt32(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsInt32_Failed, member);
            }
            return IsNumber<int>(member, style, provider, errorMessage, int.TryParse);
        }

        #endregion

        #region [====== IsUInt32 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="uint"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="uint"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<uint> IsUInt32(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsUInt32(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="uint"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="uint"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<uint> IsUInt32(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsUInt32_Failed, member);
            }
            return IsNumber<uint>(member, style, provider, errorMessage, uint.TryParse);
        }

        #endregion

        #region [====== IsInt64 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="long"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="long"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<long> IsInt64(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsInt64(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="long"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="long"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<long> IsInt64(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsInt64_Failed, member);
            }
            return IsNumber<long>(member, style, provider, errorMessage, long.TryParse);
        }

        #endregion

        #region [====== IsUInt64 ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="ulong"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<ulong> IsUInt64(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsUInt64(member, NumberStyles.Integer, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="ulong"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<ulong> IsUInt64(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsUInt64_Failed, member);
            }
            return IsNumber<ulong>(member, style, provider, errorMessage, ulong.TryParse);
        }

        #endregion

        #region [====== IsSingle ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="float"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<float> IsSingle(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsSingle(member, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="float"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<float> IsSingle(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsSingle_Failed, member);
            }
            return IsNumber<float>(member, style, provider, errorMessage, float.TryParse);
        }

        #endregion

        #region [====== IsDouble ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="double"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="double"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<double> IsDouble(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsDouble(member, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="double"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="double"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<double> IsDouble(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsDouble_Failed, member);
            }
            return IsNumber<double>(member, style, provider, errorMessage, double.TryParse);
        }

        #endregion

        #region [====== IsDecimal ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="decimal"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<decimal> IsDecimal(this Member<string> member, ErrorMessage errorMessage = null)
        {
            return IsDecimal(member, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value can be converted to a <see cref="decimal"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present
        /// in the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="provider">
        /// An object that supplies culture-specific formatting information about the <paramref name="member"/>'s value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="decimal"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        public static Member<decimal> IsDecimal(this Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage = null)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (errorMessage == null)
            {
                errorMessage = new ErrorMessage(ValidationMessages.StringMember_IsDecimal_Failed, member);
            }
            return IsNumber<decimal>(member, style, provider, errorMessage, decimal.TryParse);
        }

        #endregion

        #region [====== IsNumber ======]

        private static Member<TValue> IsNumber<TValue>(Member<string> member, NumberStyles style, IFormatProvider provider, ErrorMessage errorMessage, TryConvertDelegate<TValue> tryConvert)
        {
            var converter = new StringToNumberConverter<TValue>(tryConvert);
            Func<string, bool> constraint = value => converter.TryConvertToNumber(value, style, provider);
            Func<string, TValue> selector = value => converter.Number;

            return member.Satisfies(constraint, selector, null, errorMessage);
        }

        #endregion
    }
}
