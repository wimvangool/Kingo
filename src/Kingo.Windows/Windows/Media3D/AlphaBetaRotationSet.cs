using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    internal sealed class AlphaBetaRotationSet : EulerAngleRotationSet
    {
        private readonly AxisAngleRotation3D _alphaRotation;
        private readonly AxisAngleRotation3D _betaRotation;

        public AlphaBetaRotationSet(AxisAngleRotation3D alphaRotation, AxisAngleRotation3D betaRotation)
        {
            _alphaRotation = alphaRotation;
            _betaRotation = betaRotation;
        }

        public override Quaternion ToQuaternion()
        {
            return ToQuaternion(_betaRotation) * ToQuaternion(_alphaRotation);
        }
    }
}
