using System;
using System.Globalization;

namespace Kingo.Wpf
{
    /// <summary>
    /// Represents an angle that can be expressed in radians or degrees.
    /// </summary>
    [Serializable]
    public struct Angle : IEquatable<Angle>, IComparable<Angle>, IComparable, IFormattable
    {
        /// <summary>
        /// Represents an angle of zero radians or degrees.
        /// </summary>
        public static readonly Angle Zero = new Angle();

        private const double _RadiansToDegreesFactor = 180f;
        private readonly double _radiansFactor;

        private Angle(double radiansFactor)
        {
            _radiansFactor = radiansFactor;
        }

        /// <summary>
        /// Reduces this angle to a value in the range of [-2π, 2π] rad. (or [-360, 360] degrees).
        /// </summary>
        /// <returns>The normalized angle.</returns>
        public Angle Normalize()
        {
            return new Angle(_radiansFactor % 2);
        }

        /// <summary>
        /// Returns a strictly non-negative angle.
        /// </summary>
        /// <returns>A non-negative angle.</returns>
        public Angle Absolute()
        {
            return new Angle(Math.Abs(_radiansFactor));
        }

        #region [====== Conversion ======]

        /// <inheritdoc />
        public override string ToString()
        {            
            return ToString("G", CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var radiansFactor = (float) Math.Round(_radiansFactor, 2);
            if (radiansFactor == 0)
            {
                return ToString("0", "0");
            }
            if (radiansFactor == 1)
            {
                return ToString("π", "180");
            }
            if (radiansFactor == -1)
            {
                return ToString("-π", "-180");
            }
            var radians = radiansFactor.ToString(format, formatProvider) + "π";
            var degrees = Math.Round(ToDegrees(), 2).ToString(format, formatProvider);

            return ToString(radians, degrees);
        }

        private static string ToString(string radians, string degrees)
        {
            return $"{radians} rad. ({degrees}˚)";
        }

        /// <inheritdoc />       
        public double ToRadians()
        {
            return _radiansFactor * Math.PI;
        }

        /// <inheritdoc />       
        public double ToDegrees()
        {
            return _RadiansToDegreesFactor * _radiansFactor;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Angle" /> based on the specified value in radians.
        /// </summary>
        /// <param name="radians">Angle expressed in radians.</param>
        /// <returns>A new <see cref="Angle" />.</returns>
        public static Angle FromRadians(double radians)
        {
            return new Angle((float) (radians / Math.PI));
        }

        /// <summary>
        /// Creates and returns a new <see cref="Angle" /> based on the specified value in degrees.
        /// </summary>
        /// <param name="degrees">Angle expressed in degrees.</param>
        /// <returns>A new <see cref="Angle" />.</returns>
        public static Angle FromDegrees(double degrees)
        {
            return new Angle(degrees / _RadiansToDegreesFactor);
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
            if (obj is Angle)
            {
                return Equals((Angle) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Angle other)
        {
            return _radiansFactor.Equals(other._radiansFactor);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _radiansFactor.GetHashCode();
        }

        /// <summary>Determines whether <paramref name="left" /> is equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Angle left, Angle right)
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
        public int CompareTo(Angle other)
        {
            return _radiansFactor.CompareTo(other._radiansFactor);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is smaller than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(Angle left, Angle right)
        {
            return Comparable.IsLessThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if is smaller than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(Angle left, Angle right)
        {
            return Comparable.IsLessThanOrEqualTo(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(Angle left, Angle right)
        {
            return Comparable.IsGreaterThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(Angle left, Angle right)
        {
            return Comparable.IsGreaterThanOrEqualTo(left, right);
        }

        #endregion

        #region [====== Add & Subtract =======]

        /// <summary>
        /// Adds <paramref name="other"/> to this angle and returns the result.
        /// </summary>
        /// <param name="other">Another angle.</param>
        /// <returns>The resulting angle.</returns>
        public Angle Add(Angle other)
        {
            return new Angle(_radiansFactor + other._radiansFactor);
        }

        /// <summary>
        /// Subtracts <paramref name="other"/> from this angle and returns the result.
        /// </summary>
        /// <param name="other">Another angle.</param>
        /// <returns>The resulting angle.</returns>
        public Angle Subtract(Angle other)
        {
            return new Angle(_radiansFactor - other._radiansFactor);
        }

        /// <summary>
        /// Adds two angles and returns the result.
        /// </summary>
        /// <param name="left">Left angle.</param>
        /// <param name="right">Right angle.</param>
        /// <returns>The resulting angle.</returns>
        public static Angle operator +(Angle left, Angle right)
        {
            return left.Add(right);
        }

        /// <summary>
        /// Subtracts one angle from the other and returns the result.
        /// </summary>
        /// <param name="left">Left angle.</param>
        /// <param name="right">Right angle.</param>
        /// <returns>The resulting angle.</returns>
        public static Angle operator -(Angle left, Angle right)
        {
            return left.Subtract(right);
        }

        #endregion

        #region [====== Invert ======]

        /// <summary>
        /// Returns the inverted angle.
        /// </summary>
        /// <returns>The inverted angle.</returns>
        public Angle Invert()
        {
            return new Angle(-_radiansFactor);
        }

        /// <summary>
        /// Returns the inverted angle of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The angle to invert.</param>
        /// <returns>The inverted angle with respect to <paramref name="value"/>.</returns>
        public static Angle operator -(Angle value)
        {
            return value.Invert();
        }

        #endregion
    }
}