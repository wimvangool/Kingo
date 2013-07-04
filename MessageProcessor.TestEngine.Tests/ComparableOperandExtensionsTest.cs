using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public sealed class ComparableOperandExtensionsTest
    {
        private Mock<IScenario> _scenarioMock;
        private IOperand<int> _operand;

        [TestInitialize]
        public void Setup()
        {
            _scenarioMock = new Mock<IScenario>(MockBehavior.Strict);
            _operand = new Operand<int>(_scenarioMock.Object, 0);
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

        #region [====== IsSmallerThan ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsSmallerThan_Throws_IfOperandIsNull()
        {
            ComparableOperandExtensions.IsSmallerThan(null, 0);
        }

        [TestMethod]
        public void IsSmallerThan_Fails_IfExpressionIsNotSmaller()
        {
            ExpectFailure();

            _operand.IsSmallerThan(0);
        }

        [TestMethod]
        public void IsSmallerThan_DoesNotFail_IfExpressionIsSmaller()
        {
            _operand.IsSmallerThan(1);
        }

        #endregion

        #region [====== IsSmallerThanOrEqual ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsSmallerThanOrEqual_Throws_IfOperandIsNull()
        {
            ComparableOperandExtensions.IsSmallerThanOrEqualTo(null, 0);
        }

        [TestMethod]
        public void IsSmallerThanOrEqual_Fails_IfExpressionIsNotSmallerOrEqual()
        {
            ExpectFailure();

            _operand.IsSmallerThanOrEqualTo(-1);
        }
        
        [TestMethod]
        public void IsSmallerThanOrEqual_DoesNotFail_IfExpressionIsEqual()
        {
            _operand.IsSmallerThanOrEqualTo(0);
        }

        #endregion

        #region [====== IsGreaterThan ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsGreaterThan_Throws_IfOperandIsNull()
        {
            ComparableOperandExtensions.IsGreaterThan(null, 0);
        }

        [TestMethod]
        public void IsGreaterThan_Fails_IfExpressionIsNotGreater()
        {
            ExpectFailure();

            _operand.IsGreaterThan(0);
        }

        [TestMethod]
        public void IsGreaterThan_DoesNotFail_IfExpressionIsGreater()
        {
            _operand.IsGreaterThan(-1);
        }

        #endregion

        #region [====== IsGreaterThanOrEqual ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsGreaterThanOrEqual_Throws_IfOperandIsNull()
        {
            ComparableOperandExtensions.IsGreaterThanOrEqualTo(null, 0);
        }

        [TestMethod]
        public void IsGreaterThanOrEqual_Fails_IfExpressionIsNotGreaterOrEqual()
        {
            ExpectFailure();

            _operand.IsGreaterThanOrEqualTo(1);
        }

        [TestMethod]
        public void IsGreaterThanOrEqual_DoesNotFail_IfExpressionIsEqual()
        {
            _operand.IsGreaterThanOrEqualTo(0);
        }

        #endregion
    }
}
