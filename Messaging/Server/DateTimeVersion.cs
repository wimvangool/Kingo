using System.ComponentModel.Resources;
using System.Globalization;

namespace System.ComponentModel.Server
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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is DateTimeVersion)
            {
                return Equals((DateTimeVersion) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(DateTimeVersion other)
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
                return _value.CompareTo(((DateTimeVersion) obj)._value);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidInstanceException(obj);
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a new timestamp for the associated aggregate representing the current time.
        /// </summary>
        /// <returns>A new timestamp for the associated aggregate.</returns>       
        public DateTimeVersion Increment()
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
