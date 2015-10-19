using System;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Represents an identifier as defined by the C# language.
    /// </summary>
    public sealed class Identifier : IEquatable<Identifier>
    {
        private readonly string _value;

        private Identifier(string value)
        {
            _value = value;
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as Identifier);
        }

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
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Parses the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// A new <see cref="Identifier" /> instance or <c>null</c> if <paramref name="value"/> is <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not a valid identifier.
        /// </exception>
        public static Identifier Parse(string value)
        {
            throw new NotImplementedException();
        }        
    }
}
