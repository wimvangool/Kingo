using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class ControlModeInputBindingSpy : ControlModeInputBinding
    {
        private int _activateBindingCallCount;
        private int _deactivateBindingCallCount;

        public ControlModeInputBindingSpy()
        {
            Command = new ProjectionCameraControllerCommandSpy();
        }

        protected override void ActivateBinding()
        {
            _activateBindingCallCount++;
        }

        protected override void DeactivateBinding()
        {
            _deactivateBindingCallCount++;
        }

        public void RaiseCommandTrigger()
        {
            OnCommandTriggerRaised();
        }

        public void AssertCommandIs(IProjectionCameraControllerCommand command)
        {
            Assert.AreSame(command, Command);
        }

        public void AssertCommandParameterIs(object parameter)
        {
            Assert.AreEqual(parameter, CommandParameter);
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
