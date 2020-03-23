using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class QueryTestStubTest1 : QueryTestStubTest
    {
        #region [====== WhenRequest ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenRequest_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.WhenRequest(), false);
        }

        [TestMethod]
        public async Task WhenRequest_ReturnsWhenRequestState_IfTestEngineIsInReadyToConfigureState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                var state = test.WhenRequest();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a query of type 'IQuery<TResponse>'...", state.ToString());
            });
        }

        [TestMethod]
        public async Task WhenRequest_ReturnsWhenRequestState_IfTestEngineIsInReadyToConfigureState_And_SomeGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 2, 5);
                test.Given<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.WhenRequest());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenRequest_Throws_IfTestEngineIsInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                test.WhenRequest();
                test.WhenRequest();
            });
        }

        #endregion

        #region [====== WhenRequest().Returning<...>() ======]

        [TestMethod]
        public async Task WhenReturning_ReturnsWhenReturningState_IfTestEngineIsInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.WhenRequest().Returning<object>();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a query of type 'IQuery<Object>'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenReturning_Throws_IfTestEngineIsNotInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.WhenRequest();

                state.Returning<object>();
                state.Returning<object>();
            });
        }

        #endregion
    }
}
