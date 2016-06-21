using System;
using System.Globalization;
using Kingo.Resources;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Represents a length or non-negative distance in a coordinate system.
    /// </summary>
    public struct Length : IEquatable<Length>, IComparable, IComparable<Length>, IFormattable
    {
        /// <summary>
        /// Represents a zero length.
        /// </summary>
        public static readonly Length Zero = new Length(0f);
        private readonly float _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Length" /> structure.
        /// </summary>
        /// <param name="value">The value of this length.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is negative.
        /// </exception>
        public Length(float value)
        {
            if (value < 0)
            {
                throw NewInvalidLengthException(value);
            }
            _value = value;
        }

        private static Exception NewInvalidLengthException(float value)
        {
            var messageFormat = ExceptionMessages.Length_ValueCannotBeNegative;
            var message = string.Format(messageFormat, value);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }

        #region [====== Formatting & Conversion ======]

        /// <summary>
        /// Returns a Single-precision representation of this length.
        /// </summary>
        /// <returns></returns>
        public float ToSingle()
        {
            return _value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString("G", CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _value.ToString(format, formatProvider);
        }

        /// <summary>
        /// Implicitly converts the specified length to a single-precision value.
        /// </summary>
        /// <param name="instance">The instance to convert.</param>
        public static implicit operator float(Length instance)
        {
            return instance.ToSingle();
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (obj is Length)
            {
                return Equals((Length) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Length other)
        {
            return _value == other._value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>Determines whether <paramref name="left" /> is equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(Length left, Length right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Length left, Length right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region [====== CompareTo ======]

        int IComparable.CompareTo(object obj)
        {
            return Comparable.CompareValues(this, obj);
        }

        /// <inheritdoc />
        public int CompareTo(Length other)
        {
            return _value.CompareTo(other._value);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is less than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is less than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(Length left, Length right)
        {
            return Comparable.IsLessThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is less than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if is less than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(Length left, Length right)
        {
            return Comparable.IsLessThanOrEqualTo(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(Length left, Length right)
        {
            return Comparable.IsGreaterThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(Length left, Length right)
        {
            return Comparable.IsGreaterThanOrEqualTo(left, right);
        }

        #endregion
    }
}
