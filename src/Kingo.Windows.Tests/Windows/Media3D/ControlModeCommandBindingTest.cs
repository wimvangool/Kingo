using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class ControlModeCommandBindingTest
    {        
        private ControlModeCommandBindingSpy _inputBinding;

        [TestInitialize]
        public void Setup()
        {
            _inputBinding = new ControlModeCommandBindingSpy();
        }        

        #region [====== Activate ======]               

        [TestMethod]
        public void Activate_ActivatesBinding_IfBindingIsInDeactivatedState()
        {            
            _inputBinding.Activate(new ControlMode());

            _inputBinding.AssertActivateBindingCallCountIs(1);
        }               

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Activate_Throws_IfBindingIsInActivatedState()
        {                        
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Activate(new ControlMode());
        }

        #endregion

        #region [====== Deactivate ======]              

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deactivate_Throws_IfBindingIsInDeactivatedState()
        {                        
            _inputBinding.Deactivate();            
        }

        [TestMethod]
        public void Deactivate_DeactivatesBinding_IfBindingIsInActiveState()
        {            
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();

            _inputBinding.AssertDeactivateBindingCallCountIs(1);
        }

        #endregion        
    }
}
