using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class ControlModeInputBindingTest
    {        
        private ControlModeInputBindingSpy _inputBinding;

        [TestInitialize]
        public void Setup()
        {
            _inputBinding = new ControlModeInputBindingSpy();
        }

        #region [====== Initial State ======]

        [TestMethod]
        public void ControlMode_IsNull_IfInputBindingIsJustCreated()
        {
            _inputBinding.AssertControlModeIs(null);
        }

        #endregion

        #region [====== Activate ======]

        [TestMethod]
        public void ControlMode_IsNotNull_IfInputBindingIsActivated()
        {
            var controlMode = new ControlMode();

            _inputBinding.Activate(controlMode);
            _inputBinding.AssertControlModeIs(controlMode);
        }

        [TestMethod]
        public void ActivateBinding_IsCalled_WhenInputBindingIsActivated()
        {
            _inputBinding.AssertActivateBindingCallCountIs(0);
            _inputBinding.Activate(new ControlMode());
            _inputBinding.AssertActivateBindingCallCountIs(1);
        }

        #endregion

        #region [====== Deactivate ======]

        [TestMethod]
        public void ControlMode_IsNull_IfInputBindingIsDeactivated()
        {            
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();
            _inputBinding.AssertControlModeIs(null);
        }

        [TestMethod]
        public void DeactivateBinding_IsCalled_WhenInputBindingIsDeactivated()
        {
            _inputBinding.AssertDeactivateBindingCallCountIs(0);
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();
            _inputBinding.AssertDeactivateBindingCallCountIs(1);
        }

        #endregion
    }
}
