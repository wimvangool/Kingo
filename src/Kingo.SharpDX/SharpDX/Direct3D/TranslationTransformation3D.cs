﻿using System;
using System.Globalization;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a translation-transformation of a 3D-object.
    /// </summary>
    [Serializable]
    public struct TranslationTransformation3D : IEquatable<TranslationTransformation3D>, IFormattable, ITransformation3D
    {
        /// <summary>
        /// Represents the position at the origin.
        /// </summary>
        public static readonly TranslationTransformation3D NoTranslation = new TranslationTransformation3D();

        private readonly Vector3 _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationTransformation3D" /> struct.
        /// </summary>
        /// <param name="x">X-value of the position.</param>
        /// <param name="y">Y-value of the position.</param>
        /// <param name="z">Z-value of the position.</param>
        public TranslationTransformation3D(float x, float y, float z)
            : this(new Vector3(x, y, z)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationTransformation3D" /> struct.
        /// </summary>
        /// <param name="position">Position coordinates.</param>
        public TranslationTransformation3D(Vector3 position)
        {
            _position = position;
        }

        #region [====== TransformationMatrix ======]

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

        /// <inheritdoc />
        public Matrix TransformationMatrix
        {
            get { return Matrix.Translation(_position); }
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
            if (obj is TranslationTransformation3D)
            {
                return Equals((TranslationTransformation3D) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(TranslationTransformation3D other)
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
        public static bool operator ==(TranslationTransformation3D left, TranslationTransformation3D right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(TranslationTransformation3D left, TranslationTransformation3D right)
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
        public static implicit operator Vector3(TranslationTransformation3D value)
        {
            return value.ToVector();
        }        

        #endregion
    }
}