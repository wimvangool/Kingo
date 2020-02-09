using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class QueryTestStubTest : MicroProcessorTestStubTest<QueryTestStub>
    {
        #region [====== CreateMicroProcessorTest ======]

        protected override QueryTestStub CreateMicroProcessorTest() =>
            new QueryTestStub();

        #endregion

        #region [====== When (1) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When1_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.WhenRequest(), true);
        }

        #endregion

        #region [====== When (2) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When2_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When<object>(), true);
        }

        #endregion
    }
}
