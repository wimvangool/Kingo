using System.ComponentModel.Resources;
using System.Globalization;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents the version of a certain <see cref="IAggregate{TKey, TVersion}">aggregate</see>.
    /// </summary>
    public struct Int64Version : IAggregateVersion<Int64Version>, IComparable
    {
        private readonly long _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int64Version" /> structure.
        /// </summary>
        /// <param name="value">The value of this version.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is negative.
        /// </exception>
        public Int64Version(long value)
        {
            if (value < 0)
            {
                throw NewInvalidVersionException(value);
            }
            _value = value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Int64Version)
            {
                return Equals((Int64Version) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Int64Version other)
        {
            return _value.Equals(other._value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        int IComparable.CompareTo(object obj)
        {
            try
            {
                return _value.CompareTo(((Int64Version) obj)._value);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidInstanceException(obj);
            }
        }

        /// <inheritdoc />
        public int CompareTo(Int64Version other)
        {
            return _value.CompareTo(other._value);
        }        

        /// <summary>
        /// Returns the value of this version as a 64-bit integer value.
        /// </summary>
        /// <returns>The value of this version as a 64-bit integer value.</returns>
        public long ToInt64()
        {
            return _value;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the incremented version of this aggregate.
        /// </summary>
        /// <returns>The incremented version of this aggregate.</returns>
        /// <exception cref="OverflowException">
        /// The value of this instance is equal to <see cref="Int64.MaxValue" />.
        /// </exception>
        public Int64Version Increment()
        {
            checked
            {
                return new Int64Version(_value + 1);
            }
        }

        /// <summary>
        /// The initial version of every aggregate.
        /// </summary>
        public static readonly Int64Version Zero = new Int64Version(0);

        /// <summary>
        /// Increments the specified version and returns the result.
        /// </summary>
        /// <param name="version">The version to increment.</param>
        /// <returns>The incremented version.</returns>
        /// <exception cref="OverflowException">
        /// The value of this instance is equal to <see cref="Int64.MaxValue" />.
        /// </exception>
        public static Int64Version Increment(ref Int64Version version)
        {
            return version = version.Increment();
        }

        private static Exception NewInvalidVersionException(long value)
        {
            var messageFormat = ExceptionMessages.IntXXVersion_NegativeValue;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, value);
            return new ArgumentOutOfRangeException("value", message);
        }

        private static Exception NewInvalidInstanceException(object obj)
        {
            var messageFormat = ExceptionMessages.AggregateVersion_InvalidType;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, typeof(Int64Version), obj.GetType());
            return new ArgumentException(message, "obj");
        }

        #region [====== Operator Overloads ======]

        /// <summary>
        /// Determines whether two versions are equal.
        /// </summary>
        /// <param name="left">The left version.</param>
        /// <param name="right">The right version.</param>
        /// <returns>
        /// <c>true</c> if both versions are equal; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(Int64Version left, Int64Version right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two versions are unequal.
        /// </summary>
        /// <param name="left">The left version.</param>
        /// <param name="right">The right version.</param>
        /// <returns>
        /// <c>true</c> if both versions are unequal; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Int64Version left, Int64Version right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether one version is greater than the other.
        /// </summary>
        /// <param name="left">The left version.</param>
        /// <param name="right">The right version.</param>
        /// <returns>
        /// <c>true</c> if the left version is greater than the right; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(Int64Version left, Int64Version right)
        {
            return left._value > right._value;
        }

        /// <summary>
        /// Determines whether one version is less than the other.
        /// </summary>
        /// <param name="left">The left version.</param>
        /// <param name="right">The right version.</param>
        /// <returns>
        /// <c>true</c> if the left version is less than the right; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(Int64Version left, Int64Version right)
        {
            return left._value < right._value;
        }

        /// <summary>
        /// Determines whether one version is greater than or equal to the other.
        /// </summary>
        /// <param name="left">The left version.</param>
        /// <param name="right">The right version.</param>
        /// <returns>
        /// <c>true</c> if the left version is greater than or equal to the right; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(Int64Version left, Int64Version right)
        {
            return left._value >= right._value;
        }

        /// <summary>
        /// Determines whether one version is less than or equal to the other.
        /// </summary>
        /// <param name="left">The left version.</param>
        /// <param name="right">The right version.</param>
        /// <returns>
        /// <c>true</c> if the left version is less than or equal to the right; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(Int64Version left, Int64Version right)
        {
            return left._value <= right._value;
        }

        #endregion
    }
}
