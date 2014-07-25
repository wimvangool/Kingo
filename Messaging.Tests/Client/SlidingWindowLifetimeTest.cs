using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timer = System.Timers.Timer;

namespace System.ComponentModel.Messaging.Client
{
    [TestClass]
    public sealed class SlidingWindowLifetimeTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Lifetime_CannotHaveZeroTimeoutLength()
        {
            new SlidingWindowLifetime(TimeSpan.Zero);
        }

        [TestMethod]
        public void Lifetime_HasNotStarted_WhenJustCreated()
        {
            using (var lifetime = new SlidingWindowLifetime(TimeSpan.FromTicks(1)))
            {
                Assert.IsFalse(lifetime.HasStarted);    
            }            
        }

        [TestMethod]
        public void Lifetime_IsNotAlive_WhenJustCreated()
        {
            using (var lifetime = new SlidingWindowLifetime(TimeSpan.FromTicks(1)))
            {
                Assert.IsFalse(lifetime.IsAlive);               
            }            
        }

        [TestMethod]
        public void Lifetime_IsNotExpired_WhenJustCreated()
        {
            using (var lifetime = new SlidingWindowLifetime(TimeSpan.FromTicks(1)))
            {
                Assert.IsFalse(lifetime.IsExpired);    
            }            
        }

        [TestMethod]
        public void Lifetime_StartsAndExpiresAsExpected()
        {
            using (var lifetime = new SlidingWindowLifetime(TimeSpan.FromMilliseconds(100)))                       
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                lifetime.Expired += (s, e) => waitHandle.Set();
                lifetime.Start();

                Assert.IsTrue(lifetime.HasStarted);
                Assert.IsTrue(lifetime.IsAlive);

                if (waitHandle.Wait(TimeSpan.FromSeconds(1)))
                {
                    Assert.IsTrue(lifetime.HasStarted);                    
                    Assert.IsTrue(lifetime.IsExpired);
                    Assert.IsFalse(lifetime.IsAlive);
                    return;                    
                }
                TimerBasedLifetimeTest.FailBecauseLifetimeDidNotExpire();                          
            }
        }

        [TestMethod]
        public void Lifetime_RaisesExpiredEvent_OnlyOnce()
        {
            using (var lifetime = new SlidingWindowLifetime(TimeSpan.FromMilliseconds(1)))           
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                int raiseCount = 0;

                lifetime.Expired += (s, e) =>
                {
                    Interlocked.Increment(ref raiseCount);
                    waitHandle.Set();
                };
                lifetime.Start();

                if (waitHandle.Wait(TimeSpan.FromSeconds(1)))
                {
                    Assert.AreEqual(1, raiseCount);
                    return;
                }
                TimerBasedLifetimeTest.FailBecauseLifetimeDidNotExpire();                          
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Lifetime_CannotBeStarted_WhenItHasBeenDisposed()
        {
            var lifetime = new SlidingWindowLifetime(TimeSpan.FromTicks(1));
            lifetime.Dispose();
            lifetime.Start();
        }

        [TestMethod]
        public void Lifetime_IsExpanded_WhenAssociatedValueIsAccessed()
        {
            using (var lifetime = new SlidingWindowLifetime(TimeSpan.FromSeconds(1)))
            using (var valueAccessTimer = new Timer(100))
            using (var waitHandle = new ManualResetEventSlim(false))
            {      
                bool lifetimeWasExpiredTooSoon = false;
                const int maxElapsedCount = 15;
                int elapseCount = 0;

                // The valueAccessTimer is used to simulate value-access on a different thread,
                // every 100 milliseconds, for a total of 15 times. With a window-length of 1 second,
                // the last window will start roughly 1.5 seconds after Start() was called, and it will
                // thus expire roughly 2.5 seconds after Start() was called.
                valueAccessTimer.AutoReset = true;
                valueAccessTimer.Elapsed += (s, e) =>
                {
                    if (Interlocked.Increment(ref elapseCount) < maxElapsedCount)
                    {
                        lifetimeWasExpiredTooSoon = lifetimeWasExpiredTooSoon || lifetime.IsExpired;
                        lifetime.NotifyValueAccessed();
                    }
                };                
                lifetime.Expired += (s, e) => waitHandle.Set();
                lifetime.Start();

                valueAccessTimer.Start();                
                
                // Since the lifetime should expire at roughly 2500 milliseconds,
                // we'll wait for about 3000 milliseconds and then check whether
                // the Expired-event was raised as expected. Naturally, we also
                // check if the Expired-event wasn't raised too soon (that is, in the
                // phase where the value was accessed several times in a row).
                if (waitHandle.Wait(TimeSpan.FromMilliseconds(3000)))
                {
                    Assert.IsFalse(lifetimeWasExpiredTooSoon, "The lifetime expired too soon.");
                    return; 
                }
                TimerBasedLifetimeTest.FailBecauseLifetimeDidNotExpire();                          
            }
        }        
    }
}
