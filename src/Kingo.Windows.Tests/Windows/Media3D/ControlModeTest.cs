using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class ControlModeTest
    {
        private ControlMode _controlMode;

        [TestInitialize]
        public void Setup()
        {
            _controlMode = new ControlMode();
        }

        #region [====== Initial State ======]

        [TestMethod]
        public void Key_IsNull_IfControlModeIsJustCreated()
        {
            Assert.IsNull(_controlMode.Key);
        }

        [TestMethod]
        public void InputSource_IsNull_IfControlModeIsJustCreated()
        {
            Assert.IsNull(_controlMode.InputSource);
        }

        [TestMethod]
        public void Controller_IsNull_IfControlModeIsJustCreated()
        {
            Assert.IsNull(_controlMode.Controller);
        }

        [TestMethod]
        public void IsActivated_IsFalse_IfControlModeIsJustCreated()
        {
            Assert.IsFalse(_controlMode.IsActivated);
        }

        [TestMethod]
        public void InputBindings_IsEmpty_IfControlModeIsJustCreated()
        {
            Assert.IsNotNull(_controlMode.InputBindings);
            Assert.AreEqual(0, _controlMode.InputBindings.Count);
        }

        #endregion

        #region [====== Key ======]

        [TestMethod]
        public void KeyChanged_IsNotRaised_WhenKeyIsNotChanged()
        {
            var wasRaised = false;

            _controlMode.KeyChanged += (s, e) => wasRaised = true;
            _controlMode.Key = null;

            Assert.IsFalse(wasRaised);
        }

        [TestMethod]
        public void KeyChanged_IsRaised_WhenKeyIsChanged()
        {
            var wasRaised = false;
            var newValue = new object();

            _controlMode.KeyChanged += (s, e) =>
            {
                Assert.IsNull(e.OldValue);
                Assert.AreSame(newValue, e.NewValue);

                wasRaised = true;
            };
            _controlMode.Key = newValue;

            Assert.IsTrue(wasRaised);
        }

        #endregion

        #region [====== Activate ======]

        [TestMethod]
        public void InputSource_IsNotNull_IfControlModeIsActivated()
        {
            var inputSource = CreateInputSource();
            var controller = CreateController();

            _controlMode.Activate(inputSource, controller);

            Assert.AreSame(inputSource, _controlMode.InputSource);
        }

        [TestMethod]
        public void Controller_IsNotNull_IfControlModeIsActivated()
        {
            var inputSource = CreateInputSource();
            var controller = CreateController();

            _controlMode.Activate(inputSource, controller);

            Assert.AreSame(controller, _controlMode.Controller);
        }

        [TestMethod]
        public void IsActivated_IsTrue_IfControlModeIsActivated()
        {
            var inputSource = CreateInputSource();
            var controller = CreateController();

            _controlMode.Activate(inputSource, controller);

            Assert.IsTrue(_controlMode.IsActivated);
        }

        [TestMethod]
        public void Activate_ActivatesAllInputBindings()
        {
            var inputBindingA = new ControlModeInputBindingSpy();
            var inputBindingB = new ControlModeInputBindingSpy();

            _controlMode.InputBindings.Add(inputBindingA);
            _controlMode.InputBindings.Add(inputBindingB);
            _controlMode.Activate(CreateInputSource(), CreateController());

            inputBindingA.AssertActivateBindingCallCountIs(1);
            inputBindingB.AssertActivateBindingCallCountIs(1);
        }

        #endregion

        #region [====== Deactivate ======]

        [TestMethod]
        public void InputSource_IsNull_IfControlModeIsDeactivated()
        {
            var inputSource = CreateInputSource();
            var controller = CreateController();

            _controlMode.Activate(inputSource, controller);
            _controlMode.Deactivate();

            Assert.IsNull(_controlMode.InputSource);
        }

        [TestMethod]
        public void Controller_IsNull_IfControlModeIsDeactivated()
        {
            var inputSource = CreateInputSource();
            var controller = CreateController();

            _controlMode.Activate(inputSource, controller);
            _controlMode.Deactivate();

            Assert.IsNull(_controlMode.Controller);
        }

        [TestMethod]
        public void IsActivated_IsFalse_IfControlModeIsDeactivated()
        {
            var inputSource = CreateInputSource();
            var controller = CreateController();

            _controlMode.Activate(inputSource, controller);
            _controlMode.Deactivate();

            Assert.IsFalse(_controlMode.IsActivated);
        }

        [TestMethod]
        public void Deactivate_DeactivatesAllInputBindings()
        {
            var inputBindingA = new ControlModeInputBindingSpy();
            var inputBindingB = new ControlModeInputBindingSpy();

            _controlMode.InputBindings.Add(inputBindingA);
            _controlMode.InputBindings.Add(inputBindingB);
            _controlMode.Activate(CreateInputSource(), CreateController());
            _controlMode.Deactivate();

            inputBindingA.AssertDeactivateBindingCallCountIs(1);
            inputBindingB.AssertDeactivateBindingCallCountIs(1);
        }

        #endregion

        #region [====== Add & Remove of InputBindings ======]

        [TestMethod]
        public void Add_DoesNotActivateInputBinding_IfControlModeIsNotActivated()
        {
            var inputBinding = new ControlModeInputBindingSpy();

            _controlMode.InputBindings.Add(inputBinding);

            inputBinding.AssertActivateBindingCallCountIs(0);
        }

        [TestMethod]
        public void Add_ActivatesInputBinding_IfControlModeIsActivated()
        {
            var inputBinding = new ControlModeInputBindingSpy();

            _controlMode.Activate(CreateInputSource(), CreateController());
            _controlMode.InputBindings.Add(inputBinding);

            inputBinding.AssertActivateBindingCallCountIs(1);
        }

        [TestMethod]
        public void Remove_DoesNotDeactivateInputBinding_IfControlModeIsNotActivated()
        {
            var inputBinding = new ControlModeInputBindingSpy();

            _controlMode.InputBindings.Add(inputBinding);            

            inputBinding.AssertDeactivateBindingCallCountIs(0);
        }

        [TestMethod]
        public void Remove_DeactivatesInputBinding_IfControlModeIsActivated()
        {
            var inputBinding = new ControlModeInputBindingSpy();

            _controlMode.InputBindings.Add(inputBinding);
            _controlMode.Activate(CreateInputSource(), CreateController());
            _controlMode.InputBindings.Remove(inputBinding);

            inputBinding.AssertDeactivateBindingCallCountIs(1);
        }

        #endregion

        private static UIElement CreateInputSource()
        {
            return new Label();
        }

        private static IProjectionCameraController CreateController()
        {
            return new Mock<IProjectionCameraController>().Object;
        }
    }
}
