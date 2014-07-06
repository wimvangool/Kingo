using System;
using System.Globalization;
using YellowFlare.MessageProcessing.Clocks;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// Represents the version of a certain <see cref="IAggregate{TKey, TVersion}">aggregate</see>.
    /// </summary>
    public struct DateTimeVersion : IAggregateVersion<DateTimeVersion>, IComparable
    {
        private readonly DateTime _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeVersion" /> structure.
        /// </summary>
        /// <param name="value">The value of this version.</param>        
        public DateTimeVersion(DateTime value)
        {            
            _value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is DateTimeVersion)
            {
                return Equals((DateTimeVersion) obj);
            }
            return false;
        }

        public bool Equals(DateTimeVersion other)
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
                return _value.CompareTo(((DateTimeVersion) obj)._value);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidInstanceException(obj);
            }
        }

        public int CompareTo(DateTimeVersion other)
        {
            return _value.CompareTo(other._value);
        }        

        /// <summary>
        /// Returns the value of this version as a 64-bit integer value.
        /// </summary>
        /// <returns>The value of this version as a 64-bit integer value.</returns>
        public DateTime ToDateTime()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a new timestamp for the associated aggregate.
        /// </summary>
        /// <returns>A new timestamp for the associated aggregate.</returns>
        /// <exception cref="OverflowException">
        /// The value of this instance is equal to <see cref="DateTime.MaxValue" />.
        /// </exception>
        /// <remarks>
        /// By default, this method returns the current date and time as the new timestamp.
        /// Whether this timestamp is based on local time or UTC time depends on the timezone of
        /// this instance. However, if the current timestamp is larger than the new timestamp (in
        /// other words, it has a value from the future), the smallest possible increment is applied,
        /// which is one tick.
        /// </remarks>
        public DateTimeVersion Increment()
        {
            var newVersion = IncrementVersion();
            if (newVersion < this)
            {
                try
                {
                    return new DateTimeVersion(_value.AddTicks(1));
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    throw NewTimestampOverflowException(exception);
                }                                
            }
            return newVersion;
        }

        private DateTimeVersion IncrementVersion()
        {
            return _value.Kind == DateTimeKind.Utc ? UtcNow() : Now();            
        }

        /// <summary>
        /// Returns a timestamp with the current local date and time.
        /// </summary>
        public static DateTimeVersion Now()
        {
            return new DateTimeVersion(Clock.Current.LocalDateAndTime());
        }

        /// <summary>
        /// Returns a timestamp with the current UTC date and time.
        /// </summary>
        /// <returns></returns>
        public static DateTimeVersion UtcNow()
        {
            return new DateTimeVersion(Clock.Current.UtcDateAndTime());
        }

        /// <summary>
        /// Increments the specified version and returns the result.
        /// </summary>
        /// <param name="version">The version to increment.</param>
        /// <returns>The incremented version.</returns>
        /// <exception cref="OverflowException">
        /// The value of this instance is equal to <see cref="DateTime.MaxValue" />.
        /// </exception>
        public static DateTimeVersion Increment(ref DateTimeVersion version)
        {
            return version = version.Increment();
        }

        private static Exception NewTimestampOverflowException(ArgumentOutOfRangeException exception)
        {
            return new OverflowException(ExceptionMessages.DateTimeVersion_Overflow, exception);
        }

        private static Exception NewInvalidInstanceException(object obj)
        {
            var messageFormat = ExceptionMessages.AggregateVersion_InvalidType;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, typeof(DateTimeVersion), obj.GetType());
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
        public static bool operator ==(DateTimeVersion left, DateTimeVersion right)
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
        public static bool operator !=(DateTimeVersion left, DateTimeVersion right)
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
        public static bool operator >(DateTimeVersion left, DateTimeVersion right)
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
        public static bool operator <(DateTimeVersion left, DateTimeVersion right)
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
        public static bool operator >=(DateTimeVersion left, DateTimeVersion right)
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
        public static bool operator <=(DateTimeVersion left, DateTimeVersion right)
        {
            return left._value <= right._value;
        }

        #endregion
    }
}
