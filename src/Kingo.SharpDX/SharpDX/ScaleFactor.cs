using System;
using System.Globalization;
using Kingo.Resources;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Represents a scale-factor that is used to resize a graphical object.
    /// </summary>
    public struct ScaleFactor : IEquatable<ScaleFactor>, IComparable<ScaleFactor>, IComparable, IFormattable
    {      
        /// <summary>
        /// Represents a scale factor of 1.
        /// </summary>
        public static readonly ScaleFactor DefaultScale = new ScaleFactor();
          
        private readonly float _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleFactor" /> struct.
        /// </summary>
        /// <param name="value">The scale factor.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than or equal to <c>0</c>.
        /// </exception>
        public ScaleFactor(float value)
        {
            if (value <= 0)
            {
                throw NewInvalidValueException(value);
            }
            _value = value - 1;
        }

        private static Exception NewInvalidValueException(float value)
        {
            var messageFormat = ExceptionMessages.ScaleFactor_InvalidValue;
            var message = string.Format(messageFormat, value);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (obj is ScaleFactor)
            {
                return Equals((ScaleFactor) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(ScaleFactor other)
        {
            return _value.Equals(other._value);
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
        public static bool operator ==(ScaleFactor left, ScaleFactor right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(ScaleFactor left, ScaleFactor right)
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
        public int CompareTo(ScaleFactor other)
        {
            return _value.CompareTo(other._value);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is smaller than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(ScaleFactor left, ScaleFactor right)
        {
            return Comparable.IsLessThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if is smaller than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(ScaleFactor left, ScaleFactor right)
        {
            return Comparable.IsLessThanOrEqualTo(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(ScaleFactor left, ScaleFactor right)
        {
            return Comparable.IsGreaterThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(ScaleFactor left, ScaleFactor right)
        {
            return Comparable.IsGreaterThanOrEqualTo(left, right);
        }

        #endregion

        #region [====== Conversion ======]

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString("G", CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToSingle().ToString(format, formatProvider);
        }

        /// <summary>
        /// Returns the Single-precision value of this scale factor.
        /// </summary>
        /// <returns>The Single-precision value.</returns>
        public float ToSingle()
        {
            return _value + 1;
        }

        /// <summary>
        /// Implicitly converts the specified value to its Single-precision value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator float(ScaleFactor value)
        {
            return value.ToSingle();
        }

        #endregion

        #region [====== Multiply ======]

        /// <summary>
        /// Multiplies this scale factor with the specified <paramref name="value"/> and returns the result.
        /// </summary>
        /// <param name="value">The scale factor to mutiply with.</param>
        /// <returns>The resulting scale factor.</returns>
        public ScaleFactor Multiply(ScaleFactor value)
        {
            return new ScaleFactor(ToSingle() * value.ToSingle());
        }

        /// <summary>
        /// Multiplies two scale factors and returns the result.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The resulting scale factor.</returns>
        public static ScaleFactor operator *(ScaleFactor left, ScaleFactor right)
        {
            return left.Multiply(right);
        }

        #endregion
    }
}
