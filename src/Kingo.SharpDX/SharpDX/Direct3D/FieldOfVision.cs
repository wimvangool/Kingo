using System;
using Kingo.Resources;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents the viewing angle of a camera.
    /// </summary>
    public sealed class FieldOfVision : IEquatable<FieldOfVision>, IComparable<FieldOfVision>, IComparable, IAngle
    {
        private static readonly Angle _MinimumAngle = Angle.Zero;
        private static readonly Angle _MaximumAngle = Angle.FromDegrees(180);

        /// <summary>
        /// The default field of vision, defined as 90 degrees.
        /// </summary>
        public static readonly FieldOfVision Default = FromDegrees(90);

        private readonly Angle _angle;

        private FieldOfVision(Angle angle)
        {
            _angle = angle;
        }

        #region [====== Formatting and Parsing ======]

        /// <inheritdoc />
        public override string ToString()
        {
            return _angle.ToString();
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _angle.ToString(format, formatProvider);
        }

        /// <inheritdoc />
        public float ToRadians()
        {
            return _angle.ToRadians();
        }

        /// <inheritdoc />
        public float ToDegrees()
        {
            return _angle.ToDegrees();
        }

        /// <summary>
        /// Creates and returns a new <see cref="FieldOfVision"/> based on the specified angle, expressed in radians.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>A new <see cref="FieldOfVision"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radians"/> is outside the range of valid values.
        /// </exception>
        public static FieldOfVision FromRadians(float radians)
        {
            return FromAngle(Angle.FromRadians(radians));
        }

        /// <summary>
        /// Creates and returns a new <see cref="FieldOfVision"/> based on the specified angle, expressed in degrees.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>A new <see cref="FieldOfVision"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="degrees"/> is outside the range of valid values.
        /// </exception>
        public static FieldOfVision FromDegrees(float degrees)
        {
            return FromAngle(Angle.FromDegrees(degrees));
        }

        /// <summary>
        /// Creates and returns a new <see cref="FieldOfVision"/> based on the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>A new <see cref="FieldOfVision"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="angle"/> is outside the range of valid values.
        /// </exception>
        public static FieldOfVision FromAngle(Angle angle)
        {
            if (_MinimumAngle < angle && angle < _MaximumAngle)
            {
                return new FieldOfVision(angle);
            }
            throw NewInvalidAngleException(angle);
        }

        private static Exception NewInvalidAngleException(Angle angle)
        {
            var messageFormat = ExceptionMessages.FieldOfVision_InvalidAngle;
            var message = string.Format(messageFormat, angle, _MinimumAngle, _MaximumAngle);
            return new ArgumentOutOfRangeException(nameof(angle), message);
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as FieldOfVision);
        }

        /// <inheritdoc />
        public bool Equals(FieldOfVision other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return _angle == other._angle;
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
        public static bool operator ==(FieldOfVision left, FieldOfVision right)
        {
            return Equatable.Equals(left, right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(FieldOfVision left, FieldOfVision right)
        {
            return !Equatable.Equals(left, right);
        }

        #endregion

        #region [====== CompareTo ======]

        int IComparable.CompareTo(object obj)
        {
            return Comparable.CompareReferences(this, obj);
        }

        /// <inheritdoc />
        public int CompareTo(FieldOfVision other)
        {
            if (ReferenceEquals(other, null))
            {
                return Comparable.Greater;
            }
            if (ReferenceEquals(other, this))
            {
                return Comparable.Equal;
            }
            return _angle.CompareTo(other._angle);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is smaller than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(FieldOfVision left, FieldOfVision right)
        {
            return Comparable.IsLessThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if is smaller than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(FieldOfVision left, FieldOfVision right)
        {
            return Comparable.IsLessThanOrEqualTo(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(FieldOfVision left, FieldOfVision right)
        {
            return Comparable.IsGreaterThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(FieldOfVision left, FieldOfVision right)
        {
            return Comparable.IsGreaterThanOrEqualTo(left, right);
        }

        #endregion
    }
}
