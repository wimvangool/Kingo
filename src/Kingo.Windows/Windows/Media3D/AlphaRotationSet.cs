using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    internal sealed class AlphaRotationSet : EulerAngleRotationSet
    {
        private readonly AxisAngleRotation3D _alphaRotation;

        public AlphaRotationSet(AxisAngleRotation3D alphaRotation)
        {
            _alphaRotation = alphaRotation;
        }

        public override Quaternion ToQuaternion()
        {
            return ToQuaternion(_alphaRotation);
        }
    }
}
