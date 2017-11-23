using System;
using System.Text;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents an identifier as defined by the C# language.
    /// </summary>
    [Serializable]
    public sealed class Identifier : IEquatable<Identifier>
    {
        #region [====== Builder ======]

        /// <summary>
        /// Represents a builder that can be used to build an <see cref="Identifier"/> character by character.
        /// </summary>
        public sealed class Builder
        {
            private readonly StringBuilder _builder;

            internal Builder()
            {
                _builder = new StringBuilder();
            }

            internal Builder(int capacity)
            {
                _builder = new StringBuilder(capacity);
            }

            /// <summary>
            /// Returns the current length of the identifier.
            /// </summary>
            public int Length => _builder.Length;

            /// <summary>
            /// Attempts to append the specified <paramref name="character"/> to the identifier.
            /// </summary>
            /// <param name="character">the character to append.</param>
            /// <returns><c>true</c> if the specified <paramref name="character"/> was valid and appended; otherwise <c>false</c>.</returns>
            public bool Append(char character)
            {
                if (IsValidCharacter(character, _builder.Length))
                {
                    _builder.Append(character);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Creates and returns a new <see cref="Identifier" /> based on all appended characters.            
            /// </summary>
            /// <returns>A new <see cref="Identifier"/> instance.</returns>
            /// <exception cref="InvalidOperationException">
            /// No characters have been appended to the builder yet.
            /// </exception>
            public Identifier BuildIdentifier()
            {
                if (_builder.Length == 0)
                {
                    throw NewEmptyIdentifierException();
                }
                return new Identifier(_builder.ToString());
            }

            private static Exception NewEmptyIdentifierException() => new InvalidOperationException(ExceptionMessages.Identifier_EmptyIdentifier);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Builder"/> to build a new <see cref="Identifier" /> from a set of characters.
        /// </summary>
        /// <returns>A new <see cref="Builder"/>.</returns>
        public static Builder NewBuilder() => new Builder();

        /// <summary>
        /// Creates and returns a new <see cref="Builder"/> to build a new <see cref="Identifier" /> from a set of characters.
        /// </summary>
        /// <param name="capacity">The initial capacity of the builder; typically the expected length of the identifier.</param>
        /// <returns>A new <see cref="Builder"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is a negative number.
        /// </exception>
        public static Builder NewBuilder(int capacity) => new Builder(capacity);

        #endregion

        private readonly string _value;

        private Identifier(string value)
        {
            _value = value;
        }

        /// <inheritdoc />
        public override string ToString() => _value;

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as Identifier);

        /// <inheritdoc />
        public bool Equals(Identifier other)
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
        public override int GetHashCode() => _value.GetHashCode();

        #endregion

        #region [====== Parse ======]

        internal static readonly Identifier[] EmptyArray = new Identifier[0];

        /// <summary>
        /// Parses the specified <paramref name="value"/>, or returns <c>null</c> if <paramref name="value"/> is <c>null</c>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// A new <see cref="Identifier" /> instance or <c>null</c> if <paramref name="value"/> is <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not a valid identifier.
        /// </exception>
        public static Identifier ParseOrNull(string value) =>
            value == null ? null : Parse(value);

        /// <summary>
        /// Parses the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// A new <see cref="Identifier" /> instance based on the specified <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not a valid identifier.
        /// </exception>
        public static Identifier Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length == 0)
            {
                throw NewInvalidIdentifierException(value);
            }
            for (int index = 0; index < value.Length; index++)
            {
                if (IsValidCharacter(value[index], index))
                {
                    continue;
                }
                throw NewInvalidIdentifierException(value);
            }
            return new Identifier(value);
        }

        /// <summary>
        /// Attempts to parse the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="identifier">
        /// If this method returns <c>true</c>, this parameter will refer to the parsed <see cref="Identifier" />;
        /// otherwise <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="value"/> could be parsed to an <see cref="Identifier" />;
        /// otherwise <c>false</c>.        
        /// </returns>        
        public static bool TryParse(string value, out Identifier identifier)
        {
            if (string.IsNullOrEmpty(value))
            {
                identifier = null;
                return false;
            }
            for (int index = 0; index < value.Length; index++)
            {
                if (IsValidCharacter(value[index], index))
                {
                    continue;
                }
                identifier = null;
                return false;
            }
            identifier = new Identifier(value);
            return true;
        }

        private static bool IsValidCharacter(char character, int characterIndex) =>
            IsValidCharacter(character) || char.IsDigit(character) && characterIndex > 0;

        private static bool IsValidCharacter(char character) =>
            char.IsLetter(character) || character == '_';

        private static Exception NewInvalidIdentifierException(string value)
        {
            var messageFormat = ExceptionMessages.Identifier_InvalidIdentifier;
            var message = string.Format(messageFormat, value);
            return new ArgumentException(message, nameof(value));
        }

        #endregion

        #region [====== Operator Overloads ======]

        /// <summary>
        /// Determines whether or not <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">Left identifier.</param>
        /// <param name="right">Right identifier.</param>
        /// <returns><c>true</c> if both instances are considered equal; otherwise <c>false</c>.</returns>
        public static bool operator ==(Identifier left, Identifier right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether or not <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">Left identifier.</param>
        /// <param name="right">Right identifier.</param>
        /// <returns><c>true</c> if both instances are considered unequal; otherwise <c>false</c>.</returns>
        public static bool operator !=(Identifier left, Identifier right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            return !left.Equals(right);
        }

        /// <summary>
        /// Implicitly converts an identifier back to its string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>
        /// The string-representation of the identifier, or <c>null</c> if <paramref name="value"/> is <c>null</c>.
        /// </returns>
        public static implicit operator string(Identifier value) =>
            value == null ? null : value._value;

        #endregion
    }
}
