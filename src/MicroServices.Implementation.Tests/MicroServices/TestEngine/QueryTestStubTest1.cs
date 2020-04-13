using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kingo.Clocks;
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
        public async Task IsExecutedByQuery_Throws_IfQueryIsNull()
        {
            await RunTestAsync(test =>
            {
                test.WhenRequest().Returning<object>().IsExecutedBy(null);
            });
        }

        [TestMethod]
        public async Task IsExecutedByQuery_ReturnsExpectedState_IfConfiguratorAndQueryAreSpecified()
        {
            await RunTestAsync(test =>
            {
                var state = test.WhenRequest().Returning<object>().IsExecutedBy(new NullQuery());

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

                state.IsExecutedBy(new NullQuery());
                state.IsExecutedBy(new NullQuery());
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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewInternalServerErrorException(errorMessage);
                }, new object());

                try
                {
                    await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIs<BadRequestException>();
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
                await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfWhenOperationThrowsExceptionOfDifferentType()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw context.NewInternalServerErrorException();

                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfWhenOperationThrowsExceptionOfExpectedType_And_NoAssertMethodIsSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw context.NewBadRequestException();

                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfWhenOperationThrowsExceptionOfDerivedType_And_NoAssertMethodIsSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw context.NewNotFoundException();

                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfSpecifiedAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw context.NewBadRequestException();

                }).ThenOutputIs<BadRequestException>((exception, context) =>
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
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw context.NewBadRequestException();

                }).ThenOutputIs<BadRequestException>((exception, context) =>
                {
                    Assert.IsNotNull(exception);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw context.NewBadRequestException();

                }).ThenOutputIs<BadRequestException>();
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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewInternalServerErrorException(errorMessage);

                }, new object());

                try
                {
                    await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIsResponse();
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
                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    throw NewRandomException();

                }).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsResponse_Succeeds_IfWhenOperationDoesNotThrowException_And_AssertMethodIsNotSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIsResponse();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsResponse_Throws_IfAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIsResponse((response, context) =>
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
                var responseMessage = new object();

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    return responseMessage;

                }).ThenOutputIsResponse((response, context) =>
                {
                    Assert.AreSame(responseMessage, response);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsResponse_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.WhenRequest().Returning<object>().IsExecutedBy<NullQuery>().ThenOutputIsResponse();

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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return new object();

                }).ThenOutputIsResponse();
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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(dateOne, context.Clock.LocalDate());

                }, new object());

                test.Given().TimeIs(dateTwo);

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    AssertSameDate(dateTwo, context.Clock.LocalDate());
                    return new object();

                }).ThenOutputIsResponse();
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_IsIgnored_IfSpecifiedBeforeAllOperations_But_TimeSpanIsZero()
        {
            await RunTestAsync(async test =>
            {
                var date = Clock.SystemClock.LocalDate();

                test.Given().TimeHasPassed(TimeSpan.Zero);

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return new object();

                }).ThenOutputIsResponse();
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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return new object();

                }).ThenOutputIsResponse();
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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object());

                test.Given().TimeHasPassed(TimeSpan.Zero);

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());
                    return new object();

                }).ThenOutputIsResponse();
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

                test.Given<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(dateOne, context.Clock.LocalDate());

                }, new object());

                test.Given().TimeHasPassed(offset);

                await test.WhenRequest().Returning<object>().IsExecutedBy(context =>
                {
                    AssertSameDate(dateTwo, context.Clock.LocalDate());
                    return new object();

                }).ThenOutputIsResponse();
            });
        }

        #endregion
    }
}
