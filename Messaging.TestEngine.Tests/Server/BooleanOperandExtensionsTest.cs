using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.ComponentModel.Messaging.Server
{
    [TestClass]
    public sealed class BooleanOperandExtensionsTest
    {
        private Mock<IScenario> _scenarioMock;        

        [TestInitialize]
        public void Setup()
        {
            _scenarioMock = new Mock<IScenario>(MockBehavior.Strict);            
        }

        [TestCleanup]
        public void Teardown()
        {
            _scenarioMock.VerifyAll();
        }

        private void ExpectFailure()
        {
            _scenarioMock.Setup(framework => framework.Fail(It.IsAny<string>(), It.IsAny<object[]>()));
        }

        private IOperand<bool> CreateOperand(bool value)
        {
            return new Operand<bool>(_scenarioMock.Object, value);
        }

        #region [====== IsTrue ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsTrue_Throws_IfOperandIsNull()
        {
            BooleanOperandExtensions.IsTrue(null);
        }

        [TestMethod]
        public void IsTrue_Fails_IfExpressionIsFalse()
        {
            ExpectFailure();

            CreateOperand(false).IsTrue();
        }

        [TestMethod]
        public void IsTrue_DoesNotFail_IfExpressionIsTrue()
        {
            CreateOperand(true).IsTrue();
        }

        #endregion

        #region [====== IsFalse ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsFalse_Throws_IfOperandIsNull()
        {
            BooleanOperandExtensions.IsFalse(null);
        }

        [TestMethod]
        public void IsFalse_Fails_IfExpressionIsTrue()
        {
            ExpectFailure();

            CreateOperand(true).IsFalse();
        }

        [TestMethod]
        public void IsFalse_DoesNotFail_IfExpressionIsFalse()
        {
            CreateOperand(false).IsFalse();
        }

        #endregion
    }
}
