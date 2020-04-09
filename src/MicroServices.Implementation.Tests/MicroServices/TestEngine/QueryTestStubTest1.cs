﻿using System;
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

        #region [====== WhenRequest().Returning<...>().IsExecutedByQuery(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsExecutedByQuery_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.WhenRequest().Returning<object>().IsExecutedByQuery(null, new NullQuery());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsExecutedByQuery_Throws_IfQueryIsNull()
        {
            await RunTestAsync(test =>
            {
                test.WhenRequest().Returning<object>().IsExecutedByQuery((query, context) => { }, null);
            });
        }

        [TestMethod]
        public async Task IsExecutedByQuery_ReturnsExpectedState_IfConfiguratorAndQueryAreSpecified()
        {
            await RunTestAsync(test =>
            {
                var state = test.WhenRequest().Returning<object>().IsExecutedByQuery((query, context) => { }, new NullQuery());

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process request with query of type 'NullQuery'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task IsExecutedByQuery_Throws_IfTestEngineIsAlreadyInReadyToRunState()
        {
            await RunTestAsync(test =>
            {
                var state = test.WhenRequest().Returning<object>();

                state.IsExecutedByQuery((query, context) => { }, new NullQuery());
                state.IsExecutedByQuery((query, context) => { }, new NullQuery());
            });
        }

        #endregion

        #region [====== ThenOutputIs<...>() ======]

        //[TestMethod]
        //[ExpectedException(typeof(TestFailedException))]
        //public async Task ThenOutputIsException_Throws_IfGivenOperationThrowsException()
        //{
        //    await RunTestAsync(async test =>
        //    {
        //        var errorMessage = Guid.NewGuid().ToString();

        //        test.Given<object>().IsExecutedByCommandHandler((operation, context) =>
        //        {
        //            operation.Message = new object();
        //        }, (message, context) =>
        //        {
        //            throw context.NewInternalServerErrorException(errorMessage);
        //        });

        //        try
        //        {
        //            await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIs<BadRequestException>();
        //        }
        //        catch (TestFailedException exception)
        //        {
        //            Assert.IsInstanceOfType(exception.InnerException, typeof(InternalServerErrorException));
        //            Assert.AreEqual(errorMessage, exception.InnerException.Message);
        //            throw;
        //        }
        //    });
        //}

        #endregion
    }
}
