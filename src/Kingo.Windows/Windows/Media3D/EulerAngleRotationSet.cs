using System;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// An instance of this class represents a set of rotations that can be performed to transform one reference system into another.
    /// <a href="https://en.wikipedia.org/wiki/Euler_angles">See this article about Calculating Euler Angles</a>.
    /// </summary>
    internal abstract class EulerAngleRotationSet
    {
        public abstract Quaternion ToQuaternion();        

        public static EulerAngleRotationSet FromReferenceSystems(Vector3D x0, Vector3D z0, Vector3D x1, Vector3D z1)
        {           
            var rotationAngle = Vector3D.AngleBetween(z0, z1);

            if (IsAlmost(0, rotationAngle))
            {
                return new AlphaRotationSet(CalculateRotation(z0, x0, x1));
            }
            if (IsAlmost(180, rotationAngle))
            {
                var alphaRotation = new AxisAngleRotation3D(x0, 180);
                var betaRotation = CalculateRotation(z1, x0, x1);

                return new AlphaBetaRotationSet(alphaRotation, betaRotation);
            }
            else
            {
                var n = Vector3D.CrossProduct(z0, z1);

                var alphaRotation = CalculateRotation(z0, x0, n);
                var betaRotation = CalculateRotation(n, z0, z1);
                var gammaRotation = CalculateRotation(z1, n, x1);

                return new AlphaBetaGammaRotationSet(alphaRotation, betaRotation, gammaRotation);
            }            
        }        

        private static AxisAngleRotation3D CalculateRotation(Vector3D axisOfRotation, Vector3D from, Vector3D to)
        {
            var rotationAngle = Vector3D.AngleBetween(from, to);

            if (IsAlmost(rotationAngle, 0))
            {
                return new AxisAngleRotation3D(axisOfRotation, 0);
            }
            if (IsAlmost(rotationAngle, 180))
            {
                return new AxisAngleRotation3D(axisOfRotation, 180);
            }
            if (IsCounterClockwiseRotation(axisOfRotation, from, to))
            {
                rotationAngle = -rotationAngle;
            }
            return new AxisAngleRotation3D(axisOfRotation, rotationAngle);
        }        

        private static bool IsCounterClockwiseRotation(Vector3D axisOfRotation, Vector3D from, Vector3D to)
        {
            return Vector3D.DotProduct(Vector3D.CrossProduct(from, to), axisOfRotation) < 0;
        }       

        protected static Quaternion ToQuaternion(AxisAngleRotation3D rotation)
        {
            return new Quaternion(rotation.Axis, rotation.Angle);
        }

        internal static bool IsAlmost(double expected, double actual)
        {
            return Math.Round(actual, 6).Equals(expected);
        }
    }
}
