using System.ComponentModel.Resources;
using System.Globalization;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents the version of a certain <see cref="IAggregate{TKey, TVersion}">aggregate</see>.
    /// </summary>
    public struct Int32Version : IAggregateVersion<Int32Version>, IComparable
    {
        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32Version" /> structure.
        /// </summary>
        /// <param name="value">The value of this version.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is negative.
        /// </exception>
        public Int32Version(int value)
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
            if (obj is Int32Version)
            {
                return Equals((Int32Version) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Int32Version other)
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
                return _value.CompareTo(((Int32Version) obj)._value);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidInstanceException(obj);
            }
        }

        /// <inheritdoc />
        public int CompareTo(Int32Version other)
        {
            return _value.CompareTo(other._value);
        }        

        /// <summary>
        /// Returns the value of this version as a 32-bit integer value.
        /// </summary>
        /// <returns>The value of this version as a 32-bit integer value.</returns>
        public int ToInt32()
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
        /// The value of this instance is equal to <see cref="Int32.MaxValue" />.
        /// </exception>
        public Int32Version Increment()
        {
            checked
            {
                return new Int32Version(_value + 1);
            }
        }

        /// <summary>
        /// The initial version of every aggregate.
        /// </summary>
        public static readonly Int32Version Zero = new Int32Version(0);

        /// <summary>
        /// Increments the specified version and returns the result.
        /// </summary>
        /// <param name="version">The version to increment.</param>
        /// <returns>The incremented version.</returns>
        /// <exception cref="OverflowException">
        /// The value of this instance is equal to <see cref="Int32.MaxValue" />.
        /// </exception>
        public static Int32Version Increment(ref Int32Version version)
        {
            return version = version.Increment();
        }

        private static Exception NewInvalidVersionException(int value)
        {
            var messageFormat = ExceptionMessages.IntXXVersion_NegativeValue;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, value);
            return new ArgumentOutOfRangeException("value", message);
        }

        private static Exception NewInvalidInstanceException(object obj)
        {
            var messageFormat = ExceptionMessages.AggregateVersion_InvalidType;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, typeof(Int32Version), obj.GetType());
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
        public static bool operator ==(Int32Version left, Int32Version right)
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
        public static bool operator !=(Int32Version left, Int32Version right)
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
        public static bool operator >(Int32Version left, Int32Version right)
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
        public static bool operator <(Int32Version left, Int32Version right)
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
        public static bool operator >=(Int32Version left, Int32Version right)
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
        public static bool operator <=(Int32Version left, Int32Version right)
        {
            return left._value <= right._value;
        }

        /// <summary>
        /// Implicitly converts the specified value to a <see cref="Int32" /> instance.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An <see cref="Int32" /> instance.</returns>
        public static implicit operator int(Int32Version value)
        {
            return value._value;
        }  

        #endregion
    }
}
