using System;
using System.Globalization;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a set of rotations in a 3D coordinate system.
    /// </summary>
    [Serializable]
    public struct Rotation3D : IEquatable<Rotation3D>, IFormattable
    {        
        /// <summary>
        /// Represents no rotation.
        /// </summary>
        public static readonly Rotation3D NoRotation = FromAngles(Angle.Zero, Angle.Zero, Angle.Zero);

        private readonly Quaternion _quaternion;
        private Matrix? _rotationMatrix;
        
        private Rotation3D(Quaternion quaternion)
        {
            _quaternion = quaternion;
            _quaternion.Normalize();
            _rotationMatrix = null;
        }

        #region [====== RotationMatrix ======]

        /// <summary>
        /// Represents the (normalized) Right-vector with respect to this rotation, equivalent to the local X-axis.
        /// </summary>
        public Vector3 Right => Normalize(RotationMatrix.Right);

        /// <summary>
        /// Represents the (normalized) Up-vector with respect to this rotation, equivalent to the local Y-axis.
        /// </summary>
        public Vector3 Up => Normalize(RotationMatrix.Up);

        /// <summary>
        /// Represents the (normalized) Forward-vector with respect to this rotation, equivalent to the local Z-axis.
        /// </summary>
        public Vector3 Forward => Normalize(RotationMatrix.Backward);

        /// <summary>
        /// Returns the rotation-matrix representing the current rotation.
        /// </summary>
        public Matrix RotationMatrix
        {
            get
            {
                if (_rotationMatrix == null)
                {
                    _rotationMatrix = Matrix.RotationQuaternion(_quaternion);
                }
                return _rotationMatrix.Value;
            }
        }

        private static Vector3 Normalize(Vector3 vector)
        {
            vector.Normalize();
            return vector;
        }

        #endregion

        #region [====== Rotation with respect to Axes =====]

        /// <summary>
        /// Rotation around the x-axis.
        /// </summary>
        public Angle AroundX
        {
            get { return CalculateAngleAroundX(_quaternion.W, _quaternion.X, _quaternion.Y, _quaternion.Z); }         
        }                      

        /// <summary>
        /// Rotation around the Y-axis
        /// </summary>
        public Angle AroundY
        {
            get { return CalculateAngleAroundY(_quaternion.W, _quaternion.X, _quaternion.Y, _quaternion.Z); }
        }

        /// <summary>
        /// Rotation around the z-axis
        /// </summary>
        public Angle AroundZ
        {
            get { return CalculateAngleAroundZ(_quaternion.W, _quaternion.X, _quaternion.Y, _quaternion.Z); ; }
        }

        private static Angle CalculateAngleAroundX(float w, float x, float y, float z)
        {
            var left = 2 * (w * x + y * z);
            var right = 1 - 2 * (x * x + y * y);            

            return Angle.FromRadians((float) Math.Atan2(left, right));
        }

        private static Angle CalculateAngleAroundY(float w, float x, float y, float z)
        {                        
            return Angle.FromRadians((float) Math.Asin(2 * (w * y - x * z)));
        }

        private static Angle CalculateAngleAroundZ(float w, float x, float y, float z)
        {
            var left = 2 * (w * z + x * y);
            var right = 1 - 2 * (y * y + z * z);           

            return Angle.FromRadians((float) Math.Atan2(left, right));
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
            return $"[X = {AroundX.ToString(format, formatProvider)}][Y = {AroundY.ToString(format, formatProvider)}][Z = {AroundZ.ToString(format, formatProvider)}]";
        }        
        
        /// <summary>
        /// Creates and returns a new <see cref="Rotation3D" /> instance from the angles specified in radians.
        /// </summary>
        /// <param name="angles">The angles specified in radians.</param>
        /// <returns>A new <see cref="Rotation3D" /> instance.</returns>
        public static Rotation3D FromRadians(Vector3 angles)
        {
            return FromRadians(angles.X, angles.Y, angles.Z);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Rotation3D" /> instance from the angles specified in radians.
        /// </summary>
        /// <param name="x">Rotation around the x-axis, specified in radians.</param>
        /// <param name="y">Rotation around the y-axis, specified in radians.</param>
        /// <param name="z">Rotation around the z-axis, specified in radians.</param>
        /// <returns>A new <see cref="Rotation3D" /> instance.</returns>
        public static Rotation3D FromRadians(float x, float y, float z)
        {
            return FromAngles(Angle.FromRadians(x), Angle.FromRadians(y), Angle.FromRadians(z));
        }

        /// <summary>
        /// Creates and returns a new <see cref="Rotation3D" /> instance from the angles specified in degrees.
        /// </summary>
        /// <param name="angles">The angles specified in degrees.</param>
        /// <returns>A new <see cref="Rotation3D" /> instance.</returns>
        public static Rotation3D FromDegrees(Vector3 angles)
        {
            return FromDegrees(angles.X, angles.Y, angles.Z);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Rotation3D" /> instance from the angles specified in degrees.
        /// </summary>
        /// <param name="x">Rotation around the x-axis, specified in degrees.</param>
        /// <param name="y">Rotation around the y-axis, specified in degrees.</param>
        /// <param name="z">Rotation around the z-axis, specified in degrees.</param>
        /// <returns>A new <see cref="Rotation3D" /> instance.</returns>
        public static Rotation3D FromDegrees(float x, float y, float z)
        {
            return FromAngles(Angle.FromDegrees(x), Angle.FromDegrees(y), Angle.FromDegrees(z));
        }

        /// <summary>
        /// Creates and returns a new <see cref="Rotation3D"/> instance based on the specified angles around the x-, y- and z-azis.
        /// </summary>
        /// <param name="x">Rotation-angle around the x-axis.</param>
        /// <param name="y">Rotation-angle around the y-axis.</param>
        /// <param name="z">Rotation-angle around the z-axis.</param>
        /// <returns>A new <see cref="Rotation3D"/> instance.</returns>
        public static Rotation3D FromAngles(Angle x, Angle y, Angle z)
        {
            var rotationX = FromAngleAroundAxis(Vector3.UnitX, x);
            var rotationY = FromAngleAroundAxis(Vector3.UnitY, y);
            var rotationZ = FromAngleAroundAxis(Vector3.UnitZ, z);

            return rotationX * rotationY * rotationZ;            
        }

        /// <summary>
        /// Creates and returns a new <see cref="Rotation3D"/> instance based on a rotation around a specific axis.
        /// </summary>
        /// <param name="axis">The axis around which the rotation is performed.</param>
        /// <param name="angle">The angle of the rotation.</param>
        /// <returns>A new <see cref="Rotation3D"/> instance.</returns>
        public static Rotation3D FromAngleAroundAxis(Vector3 axis, Angle angle)
        {
            return new Rotation3D(Quaternion.RotationAxis(axis, angle.Normalize().ToRadians()));
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
            if (obj is Rotation3D)
            {
                return Equals((Rotation3D) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Rotation3D other)
        {
            return _quaternion.Equals(other._quaternion);
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
        public static bool operator ==(Rotation3D left, Rotation3D right)
        {
            return left.Equals(right);
        }

        /// <summary>Determines whether <paramref name="left" /> is not equal to <paramref name="right" />.</summary>
        /// <param name="left">Left instance</param>
        /// <param name="right">Right instance</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(Rotation3D left, Rotation3D right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region [====== Multiply ======]

        /// <summary>
        /// Combines the current rotation with the specified one, mathematically expressed as a multiplication.        
        /// </summary>
        /// <param name="right">A second rotation.</param>
        /// <returns>The combined rotation of the current and the specified one.</returns>
        public Rotation3D Multiply(Rotation3D right)
        {
            return new Rotation3D(_quaternion * right._quaternion);
        }

        /// <summary>
        /// Combines two rotations into a single rotation.
        /// </summary>
        /// <param name="left">Left rotation.</param>
        /// <param name="right">Right rotation.</param>
        /// <returns>The combined rotation.</returns>
        public static Rotation3D operator *(Rotation3D left, Rotation3D right)
        {
            return left.Multiply(right);
        }

        #endregion
    }
}
