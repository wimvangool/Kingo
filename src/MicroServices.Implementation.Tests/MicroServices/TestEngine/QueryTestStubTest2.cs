using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class QueryTestStubTest2 : QueryTestStubTest
    {
        #region [====== When<...> ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When<object>(), false);
        }

        [TestMethod]
        public async Task When_ReturnsWhenRequestState_IfTestEngineIsInReadyToConfigureState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a query of type 'IQuery<TRequest, TResponse>'...", state.ToString());
            });
        }

        [TestMethod]
        public async Task When_ReturnsWhenRequestState_IfTestEngineIsInReadyToConfigureState_And_SomeGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 2, 5);
                test.Given<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.When<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfTestEngineIsInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                test.When<object>();
                test.When<object>();
            });
        }

        #endregion

        #region [====== When<...>().Returning<...>() ======]

        [TestMethod]
        public async Task WhenReturning_ReturnsWhenReturningState_IfTestEngineIsInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>().Returning<object>();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a query of type 'IQuery<Object, Object>'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenReturning_Throws_IfTestEngineIsNotInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>();

                state.Returning<object>();
                state.Returning<object>();
            });
        }

        #endregion

        #region [====== When<...>().Returning<...>().IsExecutedByQuery(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsExecutedByQuery_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().Returning<object>().IsExecutedByQuery(null, new NullQuery());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsExecutedByQuery_Throws_IfQueryIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().Returning<object>().IsExecutedByQuery((query, context) => { }, null);
            });
        }

        [TestMethod]
        public async Task IsExecutedByQuery_ReturnsExpectedState_IfConfiguratorAndQueryAreSpecified()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>().Returning<object>().IsExecutedByQuery((query, context) => { }, new NullQuery());

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process request of type 'Object' with query of type 'NullQuery'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task IsExecutedByQuery_Throws_IfTestEngineIsAlreadyInReadyToRunState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>().Returning<object>();

                state.IsExecutedByQuery((query, context) => { }, new NullQuery());
                state.IsExecutedByQuery((query, context) => { }, new NullQuery());
            });
        }

        #endregion
    }
}
