using System;
using System.Threading.Tasks;
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
                test.Given().Message<object>();
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

                givenState.Message<object>();
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
                test.Given().Message<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    // No operation.
                });

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

                givenState.Message<object>();
                givenState.TimeHasPassed(TimeSpan.FromTicks(1));
            });
        }

        #endregion

        #region [====== Given().Message<...>(...) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Message_IsExecutedBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Message<object>().IsExecutedBy<NullHandler>(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Message_IsExecutedByCommandHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Message<object>().IsExecutedByCommandHandler(null, (message, context) => { });
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Message_IsExecutedByCommandHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Message<object>().IsExecutedByCommandHandler((operation, context) => { }, null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Message_IsHandledBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Message<object>().IsHandledBy<NullHandler>(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Message_IsHandledByCommandHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Message<object>().IsHandledByEventHandler(null, (message, context) => { });
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Given_Message_IsHandledByCommandHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.Given().Message<object>().IsHandledByEventHandler((operation, context) => { }, null);
            });
        }

        #endregion

        protected virtual async Task RunTestAsync(Action<TMicroProcessorTestStub> testMethod, bool runSetup = true, bool runTearDown = false)
        {
            var test = CreateMicroProcessorTest();

            if (runSetup)
            {
                await test.SetupAsync();
            }
            try
            {
                testMethod.Invoke(test);
            }
            finally
            {
                if (runTearDown)
                {
                    await test.TearDownAsync();
                }
            }
        }

        protected abstract TMicroProcessorTestStub CreateMicroProcessorTest();
    }
}
