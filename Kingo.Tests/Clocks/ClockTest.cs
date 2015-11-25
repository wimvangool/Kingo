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

        #region [====== CreateScope ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateScope_Throws_IfClockIsNull()
        {
            Clock.CreateScope(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateScope_Throws_IfCalledInsideAsyncLocalScope()
        {
            using (Clock.CreateAsyncLocalScope(CreateClock()))
            using (Clock.CreateScope(CreateClock()));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateScope_Throws_IfCalledInsideThreadLocalScope()
        {
            using (Clock.CreateThreadLocalScope(CreateClock()))
            using (Clock.CreateScope(CreateClock())) ;
        }

        [TestMethod]
        public void CreateScope_WillSetClockForAllThreads()
        {
            var clock = CreateClock();

            using (Clock.CreateScope(clock))
            {
                AssertIsCurrent(clock);          
                AssertIsCurrentOnOtherThread(clock);
                AssertIsCurrentOnOtherThread(clock, true);
            }                       
        }

        [TestMethod]
        public void CreateScope_IsOverriddenBy_CreateScope()
        {
            var clockA = CreateClock();
            var clockB = CreateClock();

            using (Clock.CreateScope(clockA))
            {                
                using (Clock.CreateScope(clockB))
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

        #region [====== CreateAsyncLocalScope ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateAsyncLocalScope_Throws_IfClockIsNull()
        {
            Clock.CreateAsyncLocalScope(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateAsyncLocalScope_Throws_IfCalledInsideThreadLocalScope()
        {
            using (Clock.CreateThreadLocalScope(CreateClock()))
            using (Clock.CreateAsyncLocalScope(CreateClock())) ;
        }

        [TestMethod]
        public void CreateAsyncLocalScope_WillSetClockForCurrentThread()
        {
            var clock = CreateClock();

            using (Clock.CreateAsyncLocalScope(clock))
            {
                AssertIsCurrent(clock);
                AssertIsCurrentOnOtherThread(clock);
                AssertIsNotCurrentOnOtherThread(clock, true);
            }            
        }        

        [TestMethod]
        public void CreateAsyncLocalScope_IsOverriddenBy_CreateAsyncLocalScope()
        {
            var clockA = CreateClock();
            var clockB = CreateClock();

            using (Clock.CreateAsyncLocalScope(clockA))
            {                
                using (Clock.CreateAsyncLocalScope(clockB))
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

        #region [====== CreateThreadLocalScope ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateThreadLocalScope_Throws_IfClockIsNull()
        {
            Clock.CreateThreadLocalScope(null);
        }

        [TestMethod]
        public void CreateThreadLocalScope_WillSetClockForCurrentThread()
        {
            var clock = CreateClock();

            using (Clock.CreateThreadLocalScope(clock))
            {
                AssertIsCurrent(clock);
                AssertIsNotCurrentOnOtherThread(clock);
                AssertIsNotCurrentOnOtherThread(clock, true);
            }                       
        }               

        [TestMethod]
        public void CreateThreadLocalScope_IsOverriddenBy_CreateThreadLocalScope()
        {
            var clockA = CreateClock();
            var clockB = CreateClock();

            using (Clock.CreateThreadLocalScope(clockA))
            {                
                using (Clock.CreateThreadLocalScope(clockB))
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
        public void Dispose_Throws_If_CreateScope_And_CreateScope_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.CreateScope(Clock.Default))
            using (Clock.CreateScope(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_CreateScope_And_CreateAsyncLocalScope_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.CreateScope(Clock.Default))
            using (Clock.CreateAsyncLocalScope(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_CreateScope_And_CreateThreadLocalScope_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.CreateScope(Clock.Default))
            using (Clock.CreateThreadLocalScope(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_CreateAsyncLocalScope_And_CreateAsyncLocalScope_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.CreateAsyncLocalScope(Clock.Default))
            using (Clock.CreateAsyncLocalScope(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_CreateAsyncLocalScope_And_CreateThreadLocalScope_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.CreateAsyncLocalScope(Clock.Default))
            using (Clock.CreateThreadLocalScope(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_Throws_If_CreateThreadLocalScope_And_CreateThreadLocalScope_AreNestedIncorrectly()
        {
            using (var outerScope = Clock.CreateThreadLocalScope(Clock.Default))
            using (Clock.CreateThreadLocalScope(Clock.Default))
            {
                outerScope.Dispose();
            }
        }

        #endregion

        private static IClock CreateClock()
        {
            return new DefaultClock();
        }

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
