using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.Windows.Media3D.ControlModeTest;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class ControlModeInputBindingTest
    {
        private ControlMode _controlMode;
        private ControlModeInputBindingSpy _inputBinding;

        [TestInitialize]
        public void Setup()
        {
            _controlMode = new ControlMode();
            _controlMode.InputBindings.Add(_inputBinding = new ControlModeInputBindingSpy());            
        }

        #region [====== Initial State ======]        

        [TestMethod]
        public void CommandParameter_IsNull_IfInputBindingIsJustCreated()
        {
            _inputBinding.AssertCommandParameterIs(null);
        }        

        #endregion

        #region [====== Activate ======]               

        [TestMethod]
        public void Activate_DoesNotCallActivateBinding_IfCommandIsNull()
        {
            _inputBinding.Command = null;

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.AssertActivateBindingCallCountIs(0);
        }

        [TestMethod]
        public void Activate_CallsActivateBinding_IfCommandIsNotNull()
        {            
            _inputBinding.Command = new ProjectionCameraCommandSpy();

            _controlMode.Activate(CreateInputSource(), CreateController());
            
            _inputBinding.AssertActivateBindingCallCountIs(1);
        }        

        [TestMethod]
        public void Activate_AttachesControllerToCommand_IfCommandIsNotNull()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = command;

            _controlMode.Activate(CreateInputSource(), CreateController());

            command.AssertAddCountIs(1);
        }

        #endregion

        #region [====== Deactivate ======]
       
        [TestMethod]
        public void Deactivate_DoesNotCallDeactivateBinding_IfBindingIsInUnboundState()
        {
            _inputBinding.Command = null;

            _controlMode.Activate(CreateInputSource(), CreateController());
            _controlMode.Deactivate();

            _inputBinding.AssertDeactivateBindingCallCountIs(0);
        }

        [TestMethod]
        public void Deactivate_CallsDeactivateBinding_IfBindingIsInBoundState()
        {
            _inputBinding.Command = new ProjectionCameraCommandSpy();

            _controlMode.Activate(CreateInputSource(), CreateController());
            _controlMode.Deactivate();

            _inputBinding.AssertDeactivateBindingCallCountIs(1);
        }

        [TestMethod]
        public void Deactivate_DetachesCommand_IfBindingIsInBoundState()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = command;

            _controlMode.Activate(CreateInputSource(), CreateController());
            _controlMode.Deactivate();

            command.AssertRemoveCountIs(1);
        }

        #endregion

        #region [====== CommandChange ======]

        [TestMethod]
        public void CommandChange_HasNoEffect_IfBindingIsInDeactivatedState()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = command;

            command.AssertAddCountIs(0);
        }

        [TestMethod]
        public void CommandChange_AttachesControllerToNewCommand_IfBindingIsInUnboundState_And_NewCommandIsNotNull()
        {
            var command = new ProjectionCameraCommandSpy();

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.Command = command;

            command.AssertAddCountIs(1);
        }

        [TestMethod]
        public void CommandChange_AttachesControllerToNewCommand_IfBindingIsInBoundState_And_NewCommandIsNotNull()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = new ProjectionCameraCommandSpy();

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.Command = command;

            command.AssertAddCountIs(1);
        }

        [TestMethod]
        public void CommandChange_DetachesControllerFromOldCommand_IfBindingIsInBoundState_And_CommandIsNull()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = command;

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.Command = null;

            command.AssertRemoveCountIs(1);
        }

        [TestMethod]
        public void CommandChange_DetachesControllerFromOldCommand_IfBindingIsInBoundState_And_CommandIsNotNull()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = command;

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.Command = new ProjectionCameraCommandSpy();

            command.AssertRemoveCountIs(1);
        }

        #endregion

        #region [====== Command Execution ======]

        [TestMethod]
        public void OnCommandTriggerRaised_HasNoEffect_IfBindingIsInDeactivatedState()
        {
            _inputBinding.RaiseCommandTrigger();
        }

        [TestMethod]
        public void OnCommandTriggerRaised_DoesNotExecuteCommand_IfCanExecuteReturnsFalse()
        {
            var command = new ProjectionCameraCommandSpy();

            _inputBinding.Command = command;

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.RaiseCommandTrigger();

            command.AssertExecutionCountIs(0);
        }

        [TestMethod]
        public void OnCommandTriggerRaised_ExecutesCommand_IfCanExecuteReturnsTrue()
        {
            var command = new ProjectionCameraCommandSpy()
            {
                CanExecute = true
            };

            _inputBinding.Command = command;

            _controlMode.Activate(CreateInputSource(), CreateController());

            _inputBinding.RaiseCommandTrigger();

            command.AssertExecutionCountIs(1);
        }

        #endregion        
    }
}
