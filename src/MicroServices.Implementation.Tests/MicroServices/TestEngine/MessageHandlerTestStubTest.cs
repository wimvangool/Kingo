using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class MessageHandlerTestStubTest : MicroProcessorTestStubTest<MessageHandlerTestStub>
    {
        #region [====== CreateMicroProcessorTest ======]

        protected override MessageHandlerTestStub CreateMicroProcessorTest() =>
            new MessageHandlerTestStub();

        #endregion

        #region [====== When ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When<object>(), false);
        }

        [Ignore]
        [TestMethod]
        public async Task When_SetsTestEngineToWhenMessageState_IfTestEngineIsInReadyState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.When<object>());
            });
        }

        [Ignore]
        [TestMethod]
        public async Task When_SetsTestEngineToWhenMessageState_IfTestEngineIsInReadyState_And_SomeGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 2, 5);
                test.Given().Message<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.When<object>());
            });
        }

        #endregion
    }
}
