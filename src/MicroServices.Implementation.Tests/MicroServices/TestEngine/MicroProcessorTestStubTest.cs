using System;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public abstract class MicroProcessorTestStubTest<TMicroProcessorTestStub>
        where TMicroProcessorTestStub : class, IMicroProcessorTestStub
    {
        #region [====== Given() ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_Throws_IfTestIsInNotInitializedState()
        {
            await RunTestAsync(test => test.Given(), false);
        }

        [TestMethod]
        public async Task Given_ReturnsGivenState_IfIsInInitializedState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.Given());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_Throws_IfTestIsInGivenState()
        {
            await RunTestAsync(test =>
            {
                test.Given();
                test.Given();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_Throws_IfTestIsInGivenMessageState()
        {
            await RunTestAsync(test =>
            {
                test.Given().Command<object>();
                test.Given();
            });
        }

        #endregion

        #region [====== Given().TimeHasPassedFor(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Given_TimeHasPassedFor_Throws_IfTestIsInGivenState_But_ValueIsNegative()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassedFor(-1);
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassedFor_SetsTestEngineInTimeHasPassedForState_IfTestIsInGivenState_And_ValueZero()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.Given().TimeHasPassedFor(0));
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassedFor_Ticks_KeepsTimelineInDefaultState_IfTestIsInGivenState_But_ValueIsZero()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassedFor(0).Ticks();
                test.Given().TimeIs(DateTimeOffset.UtcNow);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_TimeHasPassedFor_Ticks_SetsTimelineInRelativeTimeState_IfTestIsInGivenState_And_ValueIsPositve()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassedFor(1).Ticks();
                test.Given().TimeIs(DateTimeOffset.UtcNow);
            });
        }

        #endregion

        #region [====== Given().TimeHasPassed(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Given_TimeHasPassed_Throws_IfTestIsInGivenState_But_ValueIsNegative()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassed(new TimeSpan(-1));
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_KeepsTimelineInDefaultState_IfTestIsInGivenState_But_ValueIsZero()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassed(TimeSpan.Zero);
                test.Given().TimeIs(DateTimeOffset.UtcNow);
            });
        }

        [TestMethod]
        public async Task Given_TimeHasPassed_SetsTimelineToRelativeTime_IfTestIsInGivenState_And_ValueIsPositive()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassed(TimeSpan.FromTicks(1));
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_TimeHasPassed_Throws_IfTestIsInGivenMessageState()
        {
            await RunTestAsync(test =>
            {
                var givenState = test.Given();

                givenState.Command<object>();
                givenState.TimeHasPassed(TimeSpan.FromTicks(1));
            });
        }

        #endregion

        #region [====== Given().TimeIs(...) ======]

        [TestMethod]
        public async Task Given_TimeIs_SetsTimelineToSpecificTimeState_IfTestIsInGivenState_And_TimelineIsInDefaultState()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(DateTimeOffset.UtcNow);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Given_TimeIs_Throws_IfTestIsInGivenState_And_TimelineIsInSpecificTimeState_But_SpecifiedValueIsInPast()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 1, 1);
                test.Given().TimeIs(2019, 12, 31);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task Given_TimeIs_Throws_IfTestIsInGivenState_And_TimelineIsInSpecificTimeState_But_SpecifiedValueIsSameTime()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 1, 1);
                test.Given().TimeIs(2020, 1, 1);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_TimeIs_Throws_IfTestIsInGivenState_But_TimelineIsInRelativeTimeState_Because_MessageWasAlreadyHandled()
        {
            await RunTestAsync(test =>
            {
                // By scheduling a message-operation, we force the timeline to commit to relative time.
                // Hence, specifying a specific (absolute) time after it is not valid.
                test.Given().Command<object>().IsExecutedBy((message, context) =>
                {
                    // No operation.
                }, new object());

                test.Given().TimeIs(2020, 1, 1);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_TimeIs_Throws_IfTestIsInGivenState_But_TimelineIsInRelativeTimeState_Because_TimeWasAlreadyShifted()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeHasPassed(TimeSpan.FromTicks(1));
                test.Given().TimeIs(2020, 1, 1);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_TimeIs_Throws_IfTestIsInGivenMessageState()
        {
            await RunTestAsync(test =>
            {
                var givenState = test.Given();

                givenState.Command<object>();
                givenState.TimeHasPassed(TimeSpan.FromTicks(1));
            });
        }

        #endregion

        #region [====== Given().Command<...>(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Command_IsExecutedBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Command<object>().IsExecutedBy<NullHandler>(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Command_IsExecutedByCommandHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Command<object>().IsExecutedBy((message, context) => { }, null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Command_IsExecutedByCommandHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Command<object>().IsExecutedBy(null, (operation, context) => { });
            });
        }

        #endregion

        #region [====== Given().Event<...>(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Event_IsHandledBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Event<object>().IsHandledBy<NullHandler>(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Event_IsHandledByCommandHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Event<object>().IsHandledBy((message, context) => { }, null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Event_IsHandledByCommandHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Event<object>().IsHandledBy(null, (operation, context) => { });
            });
        }

        #endregion

        #region [====== Given().Request().Returning<...>().IsExecutedBy<...>() ======]

        [TestMethod]
        public async Task Given_Request_ReturnsGivenRequestState_IfTestEngineIsInReadyToConfigureState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.Given().Request());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_Request_Throws_IfTestEngineIsNotInReadyToConfigureTestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.Given();

                state.Request();
                state.Request();
            });
        }

        [TestMethod]
        public async Task Given_ReturningResponse1_ReturnsGivenResponseState_IfTestEngineIsInGivenRequestOfTypeState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.Given().Request().Returning<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_ReturningResponse1_Throws_IfTestEngineIsNotInGivenRequestOfTypeState()
        {
            await RunTestAsync(test =>
            {
                var state = test.Given().Request();

                state.Returning<object>();
                state.Returning<object>();
            });
        }

        [TestMethod]
        public async Task Given_Request_IsExecutedBy_SchedulesRequestOperation_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Request().Returning<object>().IsExecutedBy<NullQuery>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Request_IsExecutedByQuery_Throws_IfQueryIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Request().Returning<object>().IsExecutedBy(null);
            });
        }

        #endregion

        #region [====== Given().Request<...>().Returning<...>().IsExecutedBy<...>() ======]

        [TestMethod]
        public async Task Given_RequestOfType_ReturnsGivenRequestState_IfTestEngineIsInReadyToConfigureState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.Given().Request<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_RequestOfType_Throws_IfTestEngineIsNotInReadyToConfigureTestState()
        {
            await RunTestAsync(test =>
            {
                var state = test.Given();
                
                state.Request<object>();
                state.Request<object>();
            });
        }

        [TestMethod]
        public async Task Given_ReturningResponse2_ReturnsGivenResponseState_IfTestEngineIsInGivenRequestOfTypeState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.Given().Request<object>().Returning<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Given_ReturningResponse2_Throws_IfTestEngineIsNotInGivenRequestOfTypeState()
        {
            await RunTestAsync(test =>
            {
                var state = test.Given().Request<object>();
                    
                state.Returning<object>();
                state.Returning<object>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_RequestOfType_IsExecutedBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Request<object>().Returning<object>().IsExecutedBy<NullQuery>(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_RequestOfType_IsExecutedByQuery_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Request<object>().Returning<object>().IsExecutedBy((message, context) => message, null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_RequestOfType_IsExecutedByQuery_Throws_IfQueryIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Request<object>().Returning<object>().IsExecutedBy(null, (operation, context) => { });
            });
        }

        #endregion

        #region [====== RunTestAsync(...) ======]

        protected Task RunTestAsync(Action<TMicroProcessorTestStub> testMethod, bool runSetup = true, bool runTearDown = false) =>
            RunTestAsync(test => AsyncMethod.Run(() => testMethod.Invoke(test)), runSetup, runTearDown);

        protected virtual async Task RunTestAsync(Func<TMicroProcessorTestStub, Task> testMethod, bool runSetup = true, bool runTearDown = false)
        {
            var test = CreateMicroProcessorTest();

            if (runSetup)
            {
                await test.SetupAsync();
            }
            try
            {
                await testMethod.Invoke(test);
            }
            finally
            {
                if (runTearDown)
                {
                    await test.TearDownAsync();
                }
            }
        }

        #endregion

        protected abstract TMicroProcessorTestStub CreateMicroProcessorTest();

        protected static Exception NewRandomException() =>
            new Exception(Guid.NewGuid().ToString());

        protected static void AssertSameDate(DateTimeOffset expectedDate, DateTimeOffset actualDate)
        {
            Assert.AreEqual(expectedDate.Year, actualDate.Year);
            Assert.AreEqual(expectedDate.Month, actualDate.Month);
            Assert.AreEqual(expectedDate.Day, actualDate.Day);
        }
    }
}
