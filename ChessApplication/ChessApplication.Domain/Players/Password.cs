using System;
using System.Text.RegularExpressions;
using Kingo.BuildingBlocks;
using Kingo.ChessApplication.Resources;

namespace Kingo.ChessApplication.Players
{
    /// <summary>
    /// Represents a password.
    /// </summary>
    public sealed class Password : IEquatable<Password>
    {
        private readonly string _value;

        private Password(string value)
        {
            _value = value;
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as Password);
        }

        /// <inheritdoc />
        public bool Equals(Password other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return _value == other._value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(_value);
        }

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// Parses the specified <paramref name="value" /> and converts it into a new <see cref="Password" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <exception cref="InvalidPasswordException">
        /// <paramref name="value" /> could not be converted because it is invalid.
        /// </exception>
        public static Password Parse(string value)
        {
            if (IsNotValid(value))
            {
                throw NewInvalidPasswordException(value);
            }
            return new Password(value);
        }

        private static bool IsNotValid(string value)
        {
            return
                string.IsNullOrWhiteSpace(value) ||
                value.Length < Constraints.PasswordMinLength ||
                value.Length > Constraints.PasswordMaxLength ||
                !Regex.IsMatch(value, Constraints.PasswordRegex);
        }

        private static Exception NewInvalidPasswordException(string value)
        {            
            var messageFormat = ExceptionMessages.Password_InvalidValue;
            var message = string.Format(messageFormat, value, typeof(Password).Name);
            return new InvalidPasswordException(value, message);
        }
    }
}