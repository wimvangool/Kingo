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
        public void CommandParameter_IsNull_IfInputBindingIsJustCreated()
        {
            _inputBinding.AssertCommandParameterIs(null);
        }

        [TestMethod]
        public void ControlMode_IsNull_IfInputBindingIsJustCreated()
        {
            _inputBinding.AssertControlModeIs(null);
        }

        #endregion

        #region [====== Activate ======]

        [TestMethod]
        public void Activate_InitializesControlMode()
        {
            var controlState = new ControlMode();

            _inputBinding.Activate(controlState);
            _inputBinding.AssertControlModeIs(controlState);
        }        

        [TestMethod]
        public void Activate_DoesNotCallActivateBinding_IfCommandIsNull()
        {
            _inputBinding.Command = null;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.AssertActivateBindingCallCountIs(0);
        }

        [TestMethod]
        public void Activate_CallsActivateBinding_IfCommandIsNotNull()
        {            
            _inputBinding.Command = new ProjectionCameraControllerCommandSpy();
            _inputBinding.Activate(new ControlMode());
            _inputBinding.AssertActivateBindingCallCountIs(1);
        }        

        [TestMethod]
        public void Activate_AttachesControllerToCommand_IfCommandIsNotNull()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = command;
            _inputBinding.Activate(new ControlMode());

            command.AssertAttachCountIs(1);
        }

        #endregion

        #region [====== Deactivate ======]

        [TestMethod]
        public void Deactivate_SetsControlModeBackToNull()
        {            
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();
            _inputBinding.AssertControlModeIs(null);
        }

        [TestMethod]
        public void Deactivate_DoesNotCallDeactivateBinding_IfBindingIsInUnboundState()
        {
            _inputBinding.Command = null;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();
            _inputBinding.AssertDeactivateBindingCallCountIs(0);
        }

        [TestMethod]
        public void Deactivate_CallsDeactivateBinding_IfBindingIsInBoundState()
        {
            _inputBinding.Command = new ProjectionCameraControllerCommandSpy();
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();
            _inputBinding.AssertDeactivateBindingCallCountIs(1);
        }

        [TestMethod]
        public void Deactivate_DetachesCommand_IfBindingIsInBoundState()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = command;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Deactivate();

            command.AssertDetachCountIs(1);
        }

        #endregion

        #region [====== CommandChange ======]

        [TestMethod]
        public void CommandChange_HasNoEffect_IfBindingIsInDeactivatedState()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = command;

            command.AssertAttachCountIs(0);
        }

        [TestMethod]
        public void CommandChange_AttachesControllerToNewCommand_IfBindingIsInUnboundState_And_NewCommandIsNotNull()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Activate(new ControlMode());
            _inputBinding.Command = command;

            command.AssertAttachCountIs(1);
        }

        [TestMethod]
        public void CommandChange_AttachesControllerToNewCommand_IfBindingIsInBoundState_And_NewCommandIsNotNull()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = new ProjectionCameraControllerCommandSpy();
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Command = command;

            command.AssertAttachCountIs(1);
        }

        [TestMethod]
        public void CommandChange_DetachesControllerFromOldCommand_IfBindingIsInBoundState_And_CommandIsNull()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = command;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Command = null;

            command.AssertDetachCountIs(1);
        }

        [TestMethod]
        public void CommandChange_DetachesControllerFromOldCommand_IfBindingIsInBoundState_And_CommandIsNotNull()
        {
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = command;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.Command = new ProjectionCameraControllerCommandSpy();

            command.AssertDetachCountIs(1);
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
            var command = new ProjectionCameraControllerCommandSpy();

            _inputBinding.Command = command;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.RaiseCommandTrigger();

            command.AssertExecutionCountIs(0);
        }

        [TestMethod]
        public void OnCommandTriggerRaised_ExecutesCommand_IfCanExecuteReturnsTrue()
        {
            var command = new ProjectionCameraControllerCommandSpy()
            {
                CanExecute = true
            };

            _inputBinding.Command = command;
            _inputBinding.Activate(new ControlMode());
            _inputBinding.RaiseCommandTrigger();

            command.AssertExecutionCountIs(1);
        }

        #endregion
    }
}
