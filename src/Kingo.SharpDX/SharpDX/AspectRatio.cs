using System;
using System.Drawing;
using System.Globalization;
using Kingo.Resources;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Represents the aspect ratio of a viewport or screen, defined by its width and height properties.
    /// </summary>
    public sealed class AspectRatio : IEquatable<AspectRatio>, IComparable<AspectRatio>, IComparable, IFormattable
    {
        /// <summary>
        /// The default aspect ratio, defined as a window size of 800x600.
        /// </summary>
        public static readonly AspectRatio Default = FromScreenSize(800f, 600f);

        private readonly Length _width;
        private readonly Length _height;

        private AspectRatio(Length width, Length height)
        {
            _width = width;
            _height = height;
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as AspectRatio);
        }

        /// <inheritdoc />
        public bool Equals(AspectRatio other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return ToSingle() == other.ToSingle();
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
        public static bool operator ==(AspectRatio left, AspectRatio right)
        {
            return Equatable.Equals(left, right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(AspectRatio left, AspectRatio right)
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
        public int CompareTo(AspectRatio other)
        {
            if (ReferenceEquals(other, null))
            {
                return Comparable.Greater;
            }
            if (ReferenceEquals(other, this))
            {
                return Comparable.Equal;
            }
            return ToSingle().CompareTo(other.ToSingle());
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is smaller than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(AspectRatio left, AspectRatio right)
        {
            return Comparable.IsLessThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is smaller than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if is smaller than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(AspectRatio left, AspectRatio right)
        {
            return Comparable.IsLessThanOrEqualTo(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >(AspectRatio left, AspectRatio right)
        {
            return Comparable.IsGreaterThan(left, right);
        }

        /// <summary>Determines whether or not <paramref name="left" /> is greater than or equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance.</param>
        /// <param name="right">Right instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is greater than or equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(AspectRatio left, AspectRatio right)
        {
            return Comparable.IsGreaterThanOrEqualTo(left, right);
        }

        #endregion

        #region [====== Formatting & Conversion ======]

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString("G", CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{_width.ToString(format, formatProvider)} : {_height.ToString(format, formatProvider)} ({ToSingle().ToString(format, formatProvider)})";
        }

        /// <summary>
        /// Returns the Single-precision value of this instance.
        /// </summary>      
        public float ToSingle()
        {
            return _width.ToSingle() / _height.ToSingle();
        }

        /// <summary>
        /// Creates and returns a new <see cref="AspectRatio"/> based on the specified <paramref name="size"/>.
        /// </summary>
        /// <param name="size">Size of a screen or window.</param>
        /// <returns>A new <see cref="AspectRatio" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <see cref="Size.Width"/> or <see cref="Size.Height"/> is <c>0</c>.
        /// </exception>
        public static AspectRatio FromScreenSize(Size size)
        {
            return FromScreenSize(size.Width, size.Height);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AspectRatio"/> based on the specified <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="width">The width of a screen or window.</param>
        /// <param name="height">The height of a screen or window.</param>
        /// <returns>A new <see cref="AspectRatio" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="width"/> or <paramref name="height"/> is <c>0</c>.
        /// </exception>
        public static AspectRatio FromScreenSize(float width, float height)
        {
            return FromScreenSize(new Length(width), new Length(height));
        }

        /// <summary>
        /// Creates and returns a new <see cref="AspectRatio"/> based on the specified <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="width">The width of a screen or window.</param>
        /// <param name="height">The height of a screen or window.</param>
        /// <returns>A new <see cref="AspectRatio" /> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="width"/> or <paramref name="height"/> is <c>0</c>.
        /// </exception>
        public static AspectRatio FromScreenSize(Length width, Length height)
        {
            if (width == Length.Zero)
            {
                throw NewInvalidLengthException(nameof(width));
            }
            if (height == Length.Zero)
            {
                throw NewInvalidLengthException(nameof(height));
            }
            return new AspectRatio(width, height);
        }

        private static Exception NewInvalidLengthException(string paramName)
        {               
            return new ArgumentOutOfRangeException(paramName, ExceptionMessages.AspectRatio_InvalidLength);
        }

        /// <summary>
        /// Implicitly converts the specified ratio to its Single-precision value, or <c>0</c> if <c>null</c> is specified.
        /// </summary>
        /// <param name="value">The aspect ratio to convert.</param>
        /// <returns>
        /// The Single-precision value of the specified <paramref name="value"/>, or <c>0</c> if <paramref name="value"/> is <c>null</c>.
        /// </returns>
        public static implicit operator float(AspectRatio value)
        {
            return ReferenceEquals(value, null) ? 0f : value.ToSingle();
        }        

        #endregion
    }
}
