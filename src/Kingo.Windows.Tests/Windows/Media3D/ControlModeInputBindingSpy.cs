using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class ControlModeInputBindingSpy : ControlModeInputBinding
    {
        private int _activateBindingCallCount;
        private int _deactivateBindingCallCount;

        protected override void ActivateBinding()
        {
            _activateBindingCallCount++;
        }

        protected override void DeactivateBinding()
        {
            _deactivateBindingCallCount++;
        }

        public void AssertControlModeIs(ControlMode controlMode)
        {
            Assert.AreSame(controlMode, ControlMode);
        }

        public void AssertActivateBindingCallCountIs(int callCount)
        {
            Assert.AreEqual(callCount, _activateBindingCallCount);
        }

        public void AssertDeactivateBindingCallCountIs(int callCount)
        {
            Assert.AreEqual(callCount, _deactivateBindingCallCount);
        }
    }
}
