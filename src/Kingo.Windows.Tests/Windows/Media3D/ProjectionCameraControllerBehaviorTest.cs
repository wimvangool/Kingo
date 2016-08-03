using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    [TestClass]
    public sealed class ProjectionCameraControllerBehaviorTest
    {        
        #region [====== InputSource =====]

        [TestMethod]
        public void InputSource_ReturnsNull_IfBehaviorIsNotAttached()
        {
            var behavior = new ProjectionCameraControllerBehavior();

            Assert.IsNull(behavior.InputSource);
        }



        #endregion

        #region [====== ControlModes ======]

        [TestMethod]
        public void ControlModes_IsEmpty_IfNoControlModesHaveBeenAdded()
        {
            var behavior = new ProjectionCameraControllerBehavior();

            Assert.IsNotNull(behavior.ControlModes);
            Assert.AreEqual(0, behavior.ControlModes.Count);
        }

        #endregion
    }
}
