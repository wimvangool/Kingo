using System;
using System.Globalization;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a certain position within a specific 3D coordinate system.
    /// </summary>
    [Serializable]
    public struct Position3D : IEquatable<Position3D>, IFormattable
    {
        /// <summary>
        /// Represents the position at the origin.
        /// </summary>
        public static readonly Position3D Origin = new Position3D();

        private readonly Vector3 _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position3D" /> struct.
        /// </summary>
        /// <param name="x">X-value of the position.</param>
        /// <param name="y">Y-value of the position.</param>
        /// <param name="z">Z-value of the position.</param>
        public Position3D(float x, float y, float z)
            : this(new Vector3(x, y, z)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position3D" /> struct.
        /// </summary>
        /// <param name="position">Position coordinates.</param>
        public Position3D(Vector3 position)
        {
            _position = position;
        }

        /// <summary>
        /// Returns the translation-matrix of the current position.
        /// </summary>
        public Matrix TranslationMatrix
        {
            get { return Matrix.Translation(_position); }
        }

        /// <summary>
        /// X-value of the position.
        /// </summary>
        public float X
        {
            get { return _position.X; }
        }

        /// <summary>
        /// Y-value of the position.
        /// </summary>
        public float Y
        {
            get { return _position.Y; }
        }

        /// <summary>
        /// Z-value of the position.
        /// </summary>
        public float Z
        {
            get { return _position.Z; }
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (obj is Position3D)
            {
                return Equals((Position3D) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Position3D other)
        {
            return _position.Equals(other._position);
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
        public static bool operator ==(Position3D left, Position3D right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Position3D left, Position3D right)
        {
            return !left.Equals(right);
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
            return $"[{X.ToString(format, formatProvider)} | {Y.ToString(format, formatProvider)} | {Z.ToString(format, formatProvider)}]";
        }       

        /// <summary>
        /// Returns the Vector-representation of this position.
        /// </summary>        
        /// <returns>A vector containing the x,-, y- and z-position.</returns>
        public Vector3 ToVector()
        {
            return _position;
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to its the Vector-representation.
        /// </summary> 
        /// <param name="value">The position to convert.</param>
        public static implicit operator Vector3(Position3D value)
        {
            return value.ToVector();
        }        

        #endregion
    }
}
