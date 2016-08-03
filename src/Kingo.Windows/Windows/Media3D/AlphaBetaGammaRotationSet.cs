using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    internal sealed class AlphaBetaGammaRotationSet : EulerAngleRotationSet
    {
        private readonly AxisAngleRotation3D _alphaRotation;
        private readonly AxisAngleRotation3D _betaRotation;
        private readonly AxisAngleRotation3D _gammaRotation;

        public AlphaBetaGammaRotationSet(AxisAngleRotation3D alphaRotation, AxisAngleRotation3D betaRotation, AxisAngleRotation3D gammaRotation)
        {
            _alphaRotation = alphaRotation;
            _betaRotation = betaRotation;
            _gammaRotation = gammaRotation;
        }   

        public override Quaternion ToQuaternion()
        {                        
            return ToQuaternion(_gammaRotation) * ToQuaternion(_betaRotation) * ToQuaternion(_alphaRotation);            
        }
    }
}
