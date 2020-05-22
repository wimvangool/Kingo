using System;
using System.Threading.Tasks;
using Kingo.Clocks;
using Kingo.MicroServices.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class DataAccessTestStubTest2 : DataAccessTestStubTest
    {
        #region [====== When().Request<...> ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When().Request<object>(), false);
        }

        [TestMethod]
        public async Task When_ReturnsWhenRequestState_IfTestEngineIsInReadyToConfigureState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Request<object>();

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
                test.Given().Event<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.When().Request<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfTestEngineIsInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                test.When().Request<object>();
                test.When().Request<object>();
            });
        }

        #endregion

        #region [====== When().Request<...>().Returning<...>() ======]

        [TestMethod]
        public async Task WhenReturning_ReturnsWhenReturningState_IfTestEngineIsInWhenRequestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Request<object>().Returning<object>();

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
                var state = test.When().Request<object>();

                state.Returning<object>();
                state.Returning<object>();
            });
        }

        #endregion

        #region [====== When().Request<...>().Returning<...>().IsExecutedByQuery(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsExecutedByQuery_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Request<object>().Returning<object>().IsExecutedBy(null, new NullQuery());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task IsExecutedByQuery_Throws_IfQueryIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Request<object>().Returning<object>().IsExecutedBy(null, (operation, context) => { });
            });
        }

        [TestMethod]
        public async Task IsExecutedByQuery_ReturnsExpectedState_IfConfiguratorAndQueryAreSpecified()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Request<object>().Returning<object>().IsExecutedBy(new NullQuery(), (query, context) => { });

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
                var state = test.When().Request<object>().Returning<object>();

                state.IsExecutedBy(new NullQuery(), (query, context) => { });
                state.IsExecutedBy(new NullQuery(), (query, context) => { });
            });
        }

        #endregion

        #region [====== ThenOutputIs<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfGivenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                var errorMessage = Guid.NewGuid().ToString();

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewInternalServerErrorException(errorMessage);
                }, new object());

                try
                {
                    await test.When().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(new object()).ThenOutputIs<BadRequestException>();
                }
                catch (TestFailedException exception)
                {
                    Assert.IsInstanceOfType(exception.InnerException, typeof(InternalServerErrorException));
                    Assert.AreEqual(errorMessage, exception.InnerException.Message);
                    throw;
                }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfWhenOperationDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(new object()).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfWhenOperationThrowsExceptionOfDifferentType()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewInternalServerErrorException();

                }, new object()).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfWhenOperationThrowsExceptionOfExpectedType_And_NoAssertMethodIsSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, new object()).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfWhenOperationThrowsExceptionOfDerivedType_And_NoAssertMethodIsSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewNotFoundException();

                }, new object()).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfSpecifiedAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, new object()).ThenOutputIs<BadRequestException>((request, exception, context) =>
                {
                    throw NewRandomException();
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfSpecifiedAssertMethodDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, inputMessage).ThenOutputIs<BadRequestException>((request, exception, context) =>
                {
                    Assert.AreSame(inputMessage, request);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, new object()).ThenOutputIs<BadRequestException>();
            }, true, true);
        }

        #endregion

        #region [====== ThenOutputIsResponse ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsResponse_Throws_IfGivenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                var errorMessage = Guid.NewGuid().ToString();

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewInternalServerErrorException(errorMessage);

                }, new object());

                try
                {
                    await test.When().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(new object()).ThenOutputIsResponse();
                }
                catch (TestFailedException exception)
                {
                    Assert.IsInstanceOfType(exception.InnerException, typeof(InternalServerErrorException));
                    Assert.AreEqual(errorMessage, exception.InnerException.Message);
                    throw;
                }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsResponse_Throws_IfWhenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    throw NewRandomException();

                }, new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsResponse_Succeeds_IfWhenOperationDoesNotThrowException_And_AssertMethodIsNotSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsResponse_Throws_IfAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(new object()).ThenOutputIsResponse((message, response, context) =>
                {
                    throw NewRandomException();
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsResponse_Succeeds_IfAssertMethodDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When().Request<object>().Returning<object>().IsExecutedBy((message, context) =>
                {
                    return message;

                }, inputMessage).ThenOutputIsResponse((request, response, context) =>
                {
                    Assert.AreSame(inputMessage, request);
                    Assert.AreSame(inputMessage, response);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsResponse_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(new object()).ThenOutputIsResponse();

            }, true, true);
        }

        #endregion

        #region [====== Time ======]

        [TestMethod]
        public async Task Given_TimeIs_SetsTheTimeToTheSpecifiedTime_IfSpecifiedBeforeAllOperations()
        {
            await RunTestAsync(async test =>
            {
                var date = new DateTime(2020, 1, 1);

                test.Given().TimeIs(date);

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                await test.When().Request<object>().Returning<object>().IsExecutedBy((request, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return request;

                }, new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task Given_TimeIs_SetsTheTimeToTheSpecifiedTime_IfSpecifiedBetweenOperations()
        {
            await RunTestAsync(async test =>
            {
                var dateOne = new DateTime(2020, 1, 1);
                var dateTwo = new DateTime(2020, 1, 2);

                test.Given().TimeIs(dateOne);

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(dateOne, context.Clock.LocalDate());

                }, new object());

                test.Given().TimeIs(dateTwo);

                await test.When().Request<object>().Returning<object>().IsExecutedBy((request, context) =>
                {
                    AssertSameDate(dateTwo, context.Clock.LocalDate());
                    return request;

                }, new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_IsIgnored_IfSpecifiedBeforeAllOperations_But_TimeSpanIsZero()
        {
            await RunTestAsync(async test =>
            {
                var date = Clock.SystemClock.LocalDate();

                test.Given().TimeHasPassed(TimeSpan.Zero);

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                await test.When().Request<object>().Returning<object>().IsExecutedBy((request, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return request;

                }, new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_SimulatesThePassingOfTime_IfSpecifiedBeforeAllOperations_And_TimeSpanIsGreaterThanZero()
        {
            await RunTestAsync(async test =>
            {
                var offset = TimeSpan.FromDays(1);
                var date = Clock.SystemClock.LocalDate().Add(offset);

                test.Given().TimeHasPassed(offset);

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                await test.When().Request<object>().Returning<object>().IsExecutedBy((request, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return request;

                }, new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_IsIgnored_IfSpecifiedBetweenOperations_But_TimeSpanIsZero()
        {
            await RunTestAsync(async test =>
            {
                var offset = TimeSpan.FromDays(1);
                var date = Clock.SystemClock.LocalDate().Add(offset);

                test.Given().TimeHasPassed(offset);

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                test.Given().TimeHasPassed(TimeSpan.Zero);

                await test.When().Request<object>().Returning<object>().IsExecutedBy((request, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return request;

                }, new object()).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_SimulatesThePassingOfTime_IfSpecifiedBetweenOperations_And_TimeSpanIsGreaterThanZero()
        {
            await RunTestAsync(async test =>
            {
                var offset = TimeSpan.FromDays(1);
                var dateOne = Clock.SystemClock.LocalDate().Add(offset);
                var dateTwo = dateOne.Add(offset);

                test.Given().TimeHasPassed(offset);

                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(dateOne, context.Clock.LocalDate());

                }, new object());

                test.Given().TimeHasPassed(offset);

                await test.When().Request<object>().Returning<object>().IsExecutedBy((request, context) =>
                {
                    AssertSameDate(dateTwo, context.Clock.LocalDate());
                    return request;

                }, new object()).ThenOutputIsResponse();
            });
        }

        #endregion
    }
}
