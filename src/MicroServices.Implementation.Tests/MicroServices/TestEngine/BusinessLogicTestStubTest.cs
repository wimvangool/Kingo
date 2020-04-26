using System;
using System.Threading.Tasks;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class BusinessLogicTestStubTest : MicroProcessorTestStubTest<BusinessLogicTestStub>
    {
        #region [====== CreateMicroProcessorTest ======]

        protected override BusinessLogicTestStub CreateMicroProcessorTest() =>
            new BusinessLogicTestStub();

        #endregion

        #region [====== When().Command<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenCommand_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When().Command<object>(), false);
        }

        [TestMethod]
        public async Task WhenCommand_ReturnsWhenCommandState_IfTestEngineIsInReadyToConfigureState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Command<object>();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a command-handler of type 'IMessageHandler<Object>'...", state.ToString());
            });
        }

        [TestMethod]
        public async Task WhenCommand_ReturnsWhenCommandState_IfTestEngineIsInReadyToConfigureState_And_SomeGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 2, 5);
                test.Given().Event<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.When().Command<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenCommand_Throws_IfTestEngineIsInWhenCommandState()
        {
            await RunTestAsync(test =>
            {
                test.When().Command<object>();
                test.When().Command<object>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenCommand_Throws_IfTestEngineIsInWhenEventState()
        {
            await RunTestAsync(test =>
            {
                test.When().Event<object>();
                test.When().Command<object>();
            });
        }

        #endregion

        #region [====== When().Event<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenEvent_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When().Event<object>(), false);
        }

        [TestMethod]
        public async Task WhenEvent_ReturnsWhenEventState_IfTestEngineIsInReadyToConfigureState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Event<object>();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a event-handler of type 'IMessageHandler<Object>'...", state.ToString());
            });
        }

        [TestMethod]
        public async Task WhenEvent_ReturnsWhenEventState_IfTestEngineIsInReadyToConfigureState_And_SomeGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 2, 5);
                test.Given().Event<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.When().Event<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenEvent_Throws_IfTestEngineIsInWhenCommandState()
        {
            await RunTestAsync(test =>
            {
                test.When().Command<object>();
                test.When().Event<object>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenEvent_Throws_IfTestEngineIsInWhenEventState()
        {
            await RunTestAsync(test =>
            {
                test.When().Event<object>();
                test.When().Event<object>();
            });
        }

        #endregion

        #region [====== When().Command<...>().IsExecutedBy<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsExecutedByCommandHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Command<object>().IsExecutedBy(new NullHandler(), null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsExecutedByCommandHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Command<object>().IsExecutedBy(null, (operation, context) => { });
            });
        }

        [TestMethod]
        public async Task WhenIsExecutedByCommandHandler_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenCommandOrEventState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Command<object>().IsExecutedBy(new NullHandler(), new object());

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process message of type 'Object' with message handler of type 'NullHandler'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsExecutedBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Command<object>().IsExecutedBy<NullHandler>(null);
            });
        }

        [TestMethod]
        public async Task WhenIsExecutedBy_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenCommandOrEventState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Command<object>().IsExecutedBy<NullHandler>(new object());

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process message of type 'Object' with message handler of type 'NullHandler'...", state.ToString());
            });
        }

        #endregion

        #region [====== When().Event<...>().IsHandledBy<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsHandledByEventHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Event<object>().IsHandledBy(new NullHandler(), null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsHandledByEventHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Event<object>().IsHandledBy(null, (operation, context) => { });
            });
        }

        [TestMethod]
        public async Task WhenIsHandledByEventHandler_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenEventOrEventState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Event<object>().IsHandledBy(new NullHandler(), (operation, context) => { });

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process message of type 'Object' with message handler of type 'NullHandler'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsHandledBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When().Event<object>().IsHandledBy<NullHandler>(null);
            });
        }

        [TestMethod]
        public async Task WhenIsHandledBy_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenEventOrEventState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When().Event<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process message of type 'Object' with message handler of type 'NullHandler'...", state.ToString());
            });
        }

        #endregion

        #region [====== ThenOutputIsException(...) ======]

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
                    await test.When().Command<object>().IsExecutedBy((message, context) =>
                    {
                        throw context.NewBadRequestException();
                    }, new object()).ThenOutputIs<BadRequestException>();
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
                await test.When().Command<object>().IsExecutedBy<NullHandler>(new object()).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfWhenOperationThrowsExceptionOfDifferentType()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy((message, context) =>
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
                await test.When().Command<object>().IsExecutedBy((message, context) =>
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
                await test.When().Command<object>().IsExecutedBy((message, context) =>
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
                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, new object()).ThenOutputIs<BadRequestException>((message, exception, context) =>
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, inputMessage).ThenOutputIs<BadRequestException>((message, exception, context) =>
                {
                    Assert.AreSame(inputMessage, message);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    throw context.NewBadRequestException();

                }, new object()).ThenOutputIs<BadRequestException>();
            }, true, true);
        }

        #endregion

        #region [====== ThenOutputIsMessageStream ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsMessageStream_Throws_IfGivenOperationThrowsException()
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
                    await test.When().Command<object>().IsExecutedBy<NullHandler>(new object()).ThenOutputIsMessageStream();
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
        public async Task ThenOutputIsMessageStream_Throws_IfWhenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    throw NewRandomException();

                }, new object()).ThenOutputIsMessageStream();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsMessageStream_Succeeds_IfWhenOperationDoesNotThrowException_And_AssertMethodIsNotSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy<NullHandler>(new object()).ThenOutputIsMessageStream();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsMessageStream_Throws_IfAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy<NullHandler>(new object()).ThenOutputIsMessageStream((message, stream, context) =>
                {
                    throw NewRandomException();
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsMessageStream_Succeeds_IfAssertMethodDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    context.MessageBus.PublishEvent(new object());

                }, inputMessage).ThenOutputIsMessageStream((message, stream, context) =>
                {
                    Assert.AreSame(inputMessage, message);
                    Assert.AreEqual(1, stream.Count);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsMessageStream_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy<NullHandler>(new object()).ThenOutputIsMessageStream();

            }, true, true);
        }

        #endregion

        #region [====== ThenOutputIsEmptyStream ======]

        [TestMethod]
        public async Task ThenOutputIsEmptyStream_Succeeds_IfOperationProducesEmptyStream()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy<NullHandler>(new object()).ThenOutputIsEmptyStream();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsEmptyStream_Throws_IfOperationProducesEmptyStream()
        {
            await RunTestAsync(async test =>
            {
                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    context.MessageBus.PublishEvent(new object());
                }, new object()).ThenOutputIsEmptyStream();
            });
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object()).ThenOutputIsEmptyStream();
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(dateTwo, context.Clock.LocalDate());

                }, new object()).ThenOutputIsEmptyStream();
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object()).ThenOutputIsEmptyStream();
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object()).ThenOutputIsEmptyStream();
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(date, context.Clock.LocalDate());

                }, new object()).ThenOutputIsEmptyStream();
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

                await test.When().Command<object>().IsExecutedBy((message, context) =>
                {
                    AssertSameDate(dateTwo, context.Clock.LocalDate());

                }, new object()).ThenOutputIsEmptyStream();
            });
        }

        #endregion
    }
}
