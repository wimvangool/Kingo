using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.ComponentModel.Server
{
    [TestClass]
    public sealed class NullableOperandExtensionsTest
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

        private IOperand<int?> CreateOperand(int? value)
        {
            return new Operand<int?>(_scenarioMock.Object, value);
        }

        #region [====== HasValue Tests ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasValue_Throws_IfOperandIsNull()
        {
            NullableOperandExtensions.HasValue(null as IOperand<int?>);
        }

        [TestMethod]
        public void HasValue_Fails_IfExpressionHasNoValue()
        {
            ExpectFailure();

            Assert.IsNotNull(CreateOperand(null).HasValue());
        }

        [TestMethod]
        public void HasValue_ReturnsConjuctionStatement_IfExpressionHasValue()
        {
            Assert.IsNotNull(CreateOperand(DateTime.Now.Second).HasValue());
        }

        #endregion

        #region [====== HasNoValue Tests ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasNoValue_Throws_IfOperandIsNull()
        {
            NullableOperandExtensions.HasNoValue(null as IOperand<int?>);
        }

        [TestMethod]
        public void HasNoValue_Fails_IfExpressionHasValue()
        {
            ExpectFailure();

            CreateOperand(DateTime.Now.Second).HasNoValue();
        }

        [TestMethod]
        public void HasNoValue_DoesNotFail_IfExpressionHasValue()
        {
            CreateOperand(null).HasNoValue();
        }

        #endregion
    }
}
