using System;
using System.Globalization;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// Represents the version of a certain <see cref="IAggregate{T}">aggregate</see>.
    /// </summary>
    public struct AggregateVersion : IEquatable<AggregateVersion>, IComparable<AggregateVersion>, IComparable
    {
        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateVersion" /> class.
        /// </summary>
        /// <param name="value">The value of this version.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is negative.
        /// </exception>
        public AggregateVersion(int value)
        {
            if (value < 0)
            {
                throw NewInvalidVersionException(value);
            }
            _value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is AggregateVersion)
            {
                return Equals((AggregateVersion) obj);
            }
            return false;
        }

        public bool Equals(AggregateVersion other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        int IComparable.CompareTo(object obj)
        {
            try
            {
                return _value.CompareTo(((AggregateVersion) obj)._value);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidInstanceException(obj);
            }
        }

        public int CompareTo(AggregateVersion other)
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
        public AggregateVersion Increment()
        {
            checked
            {
                return new AggregateVersion(_value + 1);
            }
        }

        /// <summary>
        /// The initial version of every aggregate.
        /// </summary>
        public static readonly AggregateVersion Zero = new AggregateVersion(0);

        /// <summary>
        /// Increments the specified version and returns the result.
        /// </summary>
        /// <param name="version">The version to increment.</param>
        /// <returns>The incremented version.</returns>
        /// <exception cref="OverflowException">
        /// The value of this instance is equal to <see cref="Int32.MaxValue" />.
        /// </exception>
        public static AggregateVersion Increment(ref AggregateVersion version)
        {
            return version = version.Increment();
        }

        private static Exception NewInvalidVersionException(int value)
        {
            var messageFormat = ExceptionMessages.Version_NegativeValue;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, value);
            return new ArgumentOutOfRangeException("value", message);
        }

        private static Exception NewInvalidInstanceException(object obj)
        {
            var messageFormat = ExceptionMessages.Version_InvalidType;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, typeof(AggregateVersion), obj.GetType());
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
        public static bool operator ==(AggregateVersion left, AggregateVersion right)
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
        public static bool operator !=(AggregateVersion left, AggregateVersion right)
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
        public static bool operator >(AggregateVersion left, AggregateVersion right)
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
        public static bool operator <(AggregateVersion left, AggregateVersion right)
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
        public static bool operator >=(AggregateVersion left, AggregateVersion right)
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
        public static bool operator <=(AggregateVersion left, AggregateVersion right)
        {
            return left._value <= right._value;
        }

        #endregion
    }
}
