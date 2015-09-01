using System;
using System.Text.RegularExpressions;
using ServiceComponents.ChessApplication.Resources;
using ServiceComponents;

namespace ServiceComponents.ChessApplication.Players
{
    /// <summary>
    /// Represents a username.
    /// </summary>
    public sealed class Username : IEquatable<Username>
    {
        private readonly string _value;

        private Username(string value)
        {
            _value = value;
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as Username);
        }

        /// <inheritdoc />
        public bool Equals(Username other)
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
        /// Parses the specified <paramref name="value" /> and converts it into a new <see cref="Username" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <exception cref="InvalidUsernameException">
        /// <paramref name="value" /> could not be converted because it is invalid.
        /// </exception>
        public static Username Parse(string value)
        {
            if (IsNotValid(value))
            {
                throw NewInvalidUsernameException(value);
            }
            return new Username(value);
        }

        private static bool IsNotValid(string value)
        {
            return
                string.IsNullOrWhiteSpace(value) ||
                value.Length < Constraints.UsernameMinLength ||
                value.Length > Constraints.UsernameMaxLength ||
                !Regex.IsMatch(value, Constraints.UsernameRegex);
        }

        private static Exception NewInvalidUsernameException(string value)
        {            
            var messageFormat = ExceptionMessages.Username_InvalidValue;
            var message = string.Format(messageFormat, value, typeof(Username).Name);
            return new InvalidUsernameException(value, message);
        }
    }
}