using System;
using System.Globalization;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a scale-transformation of a 3D-object.
    /// </summary>
    [Serializable]
    public struct ScaleTransformation3D : IEquatable<ScaleTransformation3D>, IFormattable, ITransformation3D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransformation3D" /> struct.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="scaleFactor"/> is less than or equal to <c>0</c>.
        /// </exception>
        public ScaleTransformation3D(float scaleFactor)
            : this(new ScaleFactor(scaleFactor)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransformation3D" /> struct.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        public ScaleTransformation3D(ScaleFactor scaleFactor)
        {
            X = Y = Z = new ScaleFactor(scaleFactor);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransformation3D" /> struct.
        /// </summary>
        /// <param name="x">Scale-factor along the X-axis.</param>
        /// <param name="y">Scale-factor along the Y-axis.</param>
        /// <param name="z">Scale-factor along the Z-axis.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="x"/>, <paramref name="y"/> ir <paramref name="z"/> is less than or equal to <c>0</c>.
        /// </exception>
        public ScaleTransformation3D(float x, float y, float z)
            : this(new ScaleFactor(x), new ScaleFactor(y), new ScaleFactor(z)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTransformation3D" /> struct.
        /// </summary>
        /// <param name="x">Scale-factor along the X-axis.</param>
        /// <param name="y">Scale-factor along the Y-axis.</param>
        /// <param name="z">Scale-factor along the Z-axis.</param>
        public ScaleTransformation3D(ScaleFactor x, ScaleFactor y, ScaleFactor z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region [====== TransformationMatrix ======]

        /// <summary>
        /// The scale-factor along the X-axis.
        /// </summary>
        public ScaleFactor X
        {
            get;            
        }

        /// <summary>
        /// The scale-factor along the Y-axis.
        /// </summary>
        public ScaleFactor Y
        {
            get;           
        }

        /// <summary>
        /// The scale-factor along the Z-axis.
        /// </summary>
        public ScaleFactor Z
        {
            get;            
        }

        /// <summary>
        /// Returns the scale-matrix that represents the current scale factors.
        /// </summary>
        public Matrix TransformationMatrix
        {
            get { return Matrix.Scaling(X, Y, Z); }
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
            return $"({X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)} {Z.ToString(format, formatProvider)})";
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
            if (obj is ScaleTransformation3D)
            {
                return Equals((ScaleTransformation3D) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(ScaleTransformation3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>Determines whether <paramref name="left" /> is equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(ScaleTransformation3D left, ScaleTransformation3D right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(ScaleTransformation3D left, ScaleTransformation3D right)
        {
            return !left.Equals(right);
        }

        #endregion        
    }
}
