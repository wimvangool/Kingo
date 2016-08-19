using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Windows
{
    /// <summary>
    /// Represents a distance in a 2D or 3D coordinate system.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DistanceConverter))]
    public struct Distance : IEquatable<Distance>,  IComparable<Distance>, IComparable
    {
        /// <summary>
        /// Represents a distance of 0.
        /// </summary>
        public static readonly Distance Zero = new Distance(0.0);

        /// <summary>
        /// Represents a distance of 1.
        /// </summary>
        public static readonly Distance Unit = new Distance(1.0);

        private readonly double _value;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Distance" /> class.
        /// </summary>
        /// <param name="value">The length.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than <c>0</c>.
        /// </exception>
        public Distance(double value)
        {
            if (value < 0)
            {
                throw NewInvalidLengthException(value);
            }
            _value = value;
        }

        private static Exception NewInvalidLengthException(double value)
        {
            var messageFormat = ExceptionMessages.Distance_InvalidDistance;
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
            if (obj is Distance)
            {
                return Equals((Distance) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Distance other)
        {
            return _value.Equals(other._value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        /// <summary>Determines whether <paramref name="left" /> is equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(Distance left, Distance right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Distance left, Distance right)
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
        public int CompareTo(Distance other)
        {
            return _value.CompareTo(other._value);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is smaller than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(Distance left, Distance right)
        {
            return Comparable.IsLessThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if is smaller than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(Distance left, Distance right)
        {
            return Comparable.IsLessThanOrEqualTo(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(Distance left, Distance right)
        {
            return Comparable.IsGreaterThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(Distance left, Distance right)
        {
            return Comparable.IsGreaterThanOrEqualTo(left, right);
        }

        #endregion

        #region [====== Conversion ======]       

        /// <inheritdoc />
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <summary>
        /// Returns the value of this distance as a <see cref="Double"/>.
        /// </summary>
        /// <returns>The double value.</returns>
        public double ToDouble()
        {
            return _value;
        }

        /// <summary>
        /// Implicitly converts the specified distance to a double.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator double(Distance value)
        {
            return value.ToDouble();
        }

        #endregion
    }
}
