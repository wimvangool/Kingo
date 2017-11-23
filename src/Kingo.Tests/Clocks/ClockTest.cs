using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Clocks
{
    [TestClass]
    public sealed class ClockTest
    {
        #region [====== Setup & TearDown ======]

        private static readonly object _SyncLock = new object();
        private IClock _clock;

        [TestInitialize]
        public void Setup()
        {
            Monitor.Enter(_SyncLock);

            _clock = Clock.Current;
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                Assert.AreSame(_clock, Clock.Current, "The default clock hasn't been restored.");
            }
            finally
            {
                Monitor.Exit(_SyncLock);
            }            
        }

        #endregion

        #region [====== Override ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Override_Throws_IfClockIsNull()
        {
            Clock.Override(null as IClock);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Override_Throws_IfDelegateIsNull()
        {
            Clock.Override(null as Func<DateTimeOffset>);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Override_Throws_IfCalledInsideAsyncLocalScope()
        {
            using (Clock.OverrideAsyncLocal(CreateClock()))
            using (Clock.Override(CreateClock())) { }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Override_Throws_IfCalledInsideThreadLocalScope()
        {
            using (Clock.OverrideThreadLocal(CreateClock()))
            using (Clock.Override(CreateClock())) { }
        }

        [TestMethod]
        public void Override_WillSetClockForAllThreads()
        {
            var clock = CreateClock();

            using (Clock.Override(clock))
            {
                AssertIsCurrent(clock);          
                AssertIsCurrentOnOtherThread(clock);
                AssertIsCurrentOnOtherThread(clock, true);
            }                       
        }

        [TestMethod]
        public void Override_IsOverriddenBy_Override()
        {
            var clockA = CreateClock();
            var clockB = CreateClock();

            using (Clock.Override(clockA))
            {                
                using (Clock.Override(clockB))
                {
                    AssertIsCurrent(clockB);                  
                    AssertIsCurrentOnOtherThread(clockB);
                    AssertIsCurrentOnOtherThread(clockB, true);
                }                
                AssertIsCurrent(clockA);
                AssertIsCurrentOnOtherThread(clockA);
                AssertIsCurrentOnOtherThread(clockA, true);
            }                                
        }        

        #endregion

        #region [====== OverrideAsyncLocal ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverrideAsyncLocal_Throws_IfClockIsNull()
        {
            Clock.OverrideAsyncLocal(null as IClock);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverrideAsyncLocal_Throws_IfDelegateIsNull()
        {
            Clock.OverrideAsyncLocal(null as Func<DateTimeOffset>);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OverrideAsyncLocal_Throws_IfCalledInsideThreadLocalScope()
        {
            using (Clock.OverrideThreadLocal(CreateClock()))
            using (Clock.OverrideAsyncLocal(CreateClock())) { }
        }

        [TestMethod]
        public void OverrideAsyncLocal_WillSetClockForCurrentThread()
        {
            var clock = CreateClock();

            using (Clock.OverrideAsyncLocal(clock))
            {
                AssertIsCurrent(clock);
                AssertIsCurrentOnOtherThread(clock);
                AssertIsNotCurrentOnOtherThread(clock, true);
            }            
        }        

        [TestMethod]
        public void OverrideAsyncLocal_IsOverriddenBy_OverrideAsyncLocal()
        {
            var clockA = CreateClock();
            var clockB = CreateClock();

            using (Clock.OverrideAsyncLocal(clockA))
            {                
                using (Clock.OverrideAsyncLocal(clockB))
                {
                    AssertIsCurrent(clockB);
                    AssertIsCurrentOnOtherThread(clockB);
                    AssertIsNotCurrentOnOtherThread(clockB, true);
                }
                AssertIsCurrent(clockA);
                AssertIsCurrentOnOtherThread(clockA);
                AssertIsNotCurrentOnOtherThread(clockA, true);
            }           
        }

        #endregion

        #region [====== OverrideThreadLocal ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverrideThreadLocal_Throws_IfClockIsNull()
        {
            Clock.OverrideThreadLocal(null as IClock);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OverrideThreadLocal_Throws_IfDelegateIsNull()
        {
            Clock.OverrideThreadLocal(null as Func<DateTimeOffset>);
        }

        [TestMethod]
        public void OverrideThreadLocal_WillSetClockForCurrentThread()
        {
            var clock = CreateClock();

            using (Clock.OverrideThreadLocal(clock))
            {
                AssertIsCurrent(clock);
                AssertIsNotCurrentOnOtherThread(clock);
                AssertIsNotCurrentOnOtherThread(clock, true);
            }                       
        }               

        [TestMethod]
        public void OverrideThreadLocal_IsOverriddenBy_OverrideThreadLocal()
        {
            var clockA = CreateClock();
            var clockB = CreateClock();

            using (Clock.OverrideThreadLocal(clockA))
            {                
                using (Clock.OverrideThreadLocal(clockB))
                {
                    AssertIsCurrent(clockB);
                    AssertIsNotCurrentOnOtherThread(clockB);
                    AssertIsNotCurrentOnOtherThread(clockB, true);
                }
                AssertIsCurrent(clockA);
                AssertIsNotCurrentOnOtherThread(clockA);
                AssertIsNotCurrentOnOtherThread(clockA, true);
            }            
        }

        #endregion

        #region [====== Dispose ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_Override_And_Override_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.Override(Clock.Default))
            using (Clock.Override(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_Override_And_OverrideAsyncLocal_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.Override(Clock.Default))
            using (Clock.OverrideAsyncLocal(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_Override_And_OverrideThreadLocal_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.Override(Clock.Default))
            using (Clock.OverrideThreadLocal(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_OverrideAsyncLocal_And_OverrideAsyncLocal_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.OverrideAsyncLocal(Clock.Default))
            using (Clock.OverrideAsyncLocal(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_OverrideAsyncLocal_And_OverrideThreadLocal_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.OverrideAsyncLocal(Clock.Default))
            using (Clock.OverrideThreadLocal(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_OverrideThreadLocal_And_OverrideThreadLocal_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.OverrideThreadLocal(Clock.Default))
            using (Clock.OverrideThreadLocal(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        #endregion

        private static IClock CreateClock() => new DefaultClock();

        private static void AssertIsCurrent(IClock clock)
        {
            Assert.AreSame(clock, Clock.Current);
        }

        private static void AssertIsCurrentOnOtherThread(IClock clock, bool suppressFlow = false)
        {
            Assert.AreSame(clock, GetClockFromOtherThread(suppressFlow));
        }

        private static void AssertIsNotCurrentOnOtherThread(IClock clock, bool suppressFlow = false)
        {
            Assert.AreNotSame(clock, GetClockFromOtherThread(suppressFlow));
        }

        private static IClock GetClockFromOtherThread(bool suppressFlow)
        {
            if (suppressFlow)
            {
                using (ExecutionContext.SuppressFlow())
                {
                    return GetClockFromOtherThread();
                }
            }
            return GetClockFromOtherThread();
        }

        private static IClock GetClockFromOtherThread()
        {
            IClock clock = null;

            using (var waitHandle = new ManualResetEventSlim())
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        clock = Clock.Current;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
                thread.Start();

                waitHandle.Wait();
            }
            return clock;
        }
    }
}
