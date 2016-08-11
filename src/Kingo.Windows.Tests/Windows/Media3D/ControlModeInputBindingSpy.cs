using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class ControlModeInputBindingSpy : ControlModeInputBinding
    {
        private int _activateBindingCallCount;
        private int _deactivateBindingCallCount;

        public ControlModeInputBindingSpy()
        {
            Command = new ProjectionCameraCommandSpy();
        }

        protected override void ActivateBinding(UIElement inputSource)
        {
            _activateBindingCallCount++;
        }

        protected override void DeactivateBinding(UIElement inputSource)
        {
            _deactivateBindingCallCount++;
        }

        public void RaiseCommandTrigger()
        {
            OnCommandTriggerRaised();
        }

        public void AssertCommandIs(IProjectionCameraCommand command)
        {
            Assert.AreSame(command, Command);
        }

        public void AssertCommandParameterIs(object parameter)
        {
            Assert.AreEqual(parameter, CommandParameter);
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
