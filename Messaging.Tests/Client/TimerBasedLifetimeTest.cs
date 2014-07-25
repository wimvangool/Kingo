using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Client
{
    [TestClass]
    public sealed class TimerBasedLifetimeTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Lifetime_CannotHaveZeroTimeoutLength()
        {
            new TimerBasedLifetime(TimeSpan.Zero);
        }

        [TestMethod]
        public void Lifetime_HasNotStarted_WhenJustCreated()
        {
            using (var lifetime = new TimerBasedLifetime(TimeSpan.FromTicks(1)))
            {
                Assert.IsFalse(lifetime.HasStarted);    
            }            
        }

        [TestMethod]
        public void Lifetime_IsNotAlive_WhenJustCreated()
        {
            using (var lifetime = new TimerBasedLifetime(TimeSpan.FromTicks(1)))
            {
                Assert.IsFalse(lifetime.IsAlive);               
            }            
        }

        [TestMethod]
        public void Lifetime_IsNotExpired_WhenJustCreated()
        {
            using (var lifetime = new TimerBasedLifetime(TimeSpan.FromTicks(1)))
            {
                Assert.IsFalse(lifetime.IsExpired);    
            }            
        }

        [TestMethod]
        public void Lifetime_StartsAndExpiresAsExpected()
        {
            using (var lifetime = new TimerBasedLifetime(TimeSpan.FromMilliseconds(100)))                       
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
                FailBecauseLifetimeDidNotExpire();                        
            }
        }

        [TestMethod]
        public void Lifetime_RaisesExpiredEvent_OnlyOnce()
        {
            using (var lifetime = new TimerBasedLifetime(TimeSpan.FromMilliseconds(1)))           
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
                FailBecauseLifetimeDidNotExpire();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Lifetime_CannotBeStarted_WhenItHasBeenDisposed()
        {
            var lifetime = new TimerBasedLifetime(TimeSpan.FromTicks(1));
            lifetime.Dispose();
            lifetime.Start();
        }

        internal static void FailBecauseLifetimeDidNotExpire()
        {
            Assert.Fail("Lifetime did not expire in a timely fashion.");
        }
    }
}
