using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class ControlModeCommandBindingSpy : ControlModeCommandBinding
    {
        private int _activateBindingCallCount;
        private int _deactivateBindingCallCount;
        
        protected override void OnActivated()
        {
            _activateBindingCallCount++;
        }

        protected override void OnDeactivating()
        {
            _deactivateBindingCallCount++;
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
