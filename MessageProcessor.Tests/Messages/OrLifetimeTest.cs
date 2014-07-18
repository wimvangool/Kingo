using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YellowFlare.MessageProcessing.Messages
{
    [TestClass]
    public sealed class OrLifetimeTest
    {
        #region [====== InfiniteLifetime Or [x] ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InfiniteLifetime_Or_Null_Throws()
        {
            using (var lifetime = new InfiniteLifetime())
            {
                lifetime.Or(null);
            }
        }

        [TestMethod]
        public void InfiniteLifetime_Or_Self_IsSameInfiniteLifetime()
        {
            using (var lifetime = new InfiniteLifetime())
            {
                Assert.AreSame(lifetime, lifetime.Or(lifetime));
            }
        }

        [TestMethod]
        public void InfiniteLifetime_Or_InfiniteLifetime_IsSameInfiniteLifetime()
        {
            using (var lifetimeA = new InfiniteLifetime())
            using (var lifetimeB = new InfiniteLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                Assert.AreSame(lifetimeC, lifetimeA);
            }
        }

        [TestMethod]
        public void InfiniteLifetime_Or_CustomLifetime_IsSameCustomLifetime()
        {
            using (var lifetimeA = new InfiniteLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                Assert.AreSame(lifetimeB, lifetimeC);
            }
        }

        #endregion        

        #region [====== CustomLifetime Or [x] ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomLifetime_Or_Null_Throws()
        {
            using (var lifetime = TimerBasedLifetime.DefaultLifetime())
            {
                lifetime.Or(null);
            }
        }

        [TestMethod]
        public void CustomLifetime_Or_Self_IsSameCustomLifetime()
        {
            using (var lifetime = TimerBasedLifetime.DefaultLifetime())
            {
                Assert.AreSame(lifetime, lifetime.Or(lifetime));
            }
        }

        [TestMethod]
        public void CustomLifetime_Or_InfiniteLifetime_IsSameCustomLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = new InfiniteLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                Assert.AreSame(lifetimeA, lifetimeC);
            }
        }

        [TestMethod]
        public void CustomLifetime_Or_CustomLifetime_IsNewOrLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                Assert.IsTrue(lifetimeC is OrLifetime);
            }
        }

        [TestMethod]
        public void CustomLifetime_Or_OrLifetime_IsNewOrLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeD = lifetimeB.Or(lifetimeC))
            using (var lifetimeE = lifetimeA.Or(lifetimeD))
            {
                Assert.IsTrue(lifetimeE is OrLifetime);
                Assert.AreNotSame(lifetimeD, lifetimeE);
            }
        }

        #endregion

        #region [====== OrLifetime Or [x] ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrLifetime_Or_Null_Throws()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                lifetimeC.Or(null);
            }
        }

        [TestMethod]
        public void OrLifetime_Or_Self_IsSameOrLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                Assert.AreSame(lifetimeC, lifetimeC.Or(lifetimeC));
            }
        }

        [TestMethod]
        public void OrLifetime_Or_InfiniteLifetime_IsSameOrLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))                      
            using (var lifetimeD = new InfiniteLifetime())
            using (var lifetimeE = lifetimeC.Or(lifetimeD))
            {
                Assert.AreSame(lifetimeC, lifetimeE);
            }
        }

        [TestMethod]
        public void OrLifetime_Or_CustomLifetime_IsNewOrLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            using (var lifetimeD = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeE = lifetimeC.Or(lifetimeD))
            {                
                Assert.IsTrue(lifetimeE is OrLifetime);
                Assert.AreNotSame(lifetimeC, lifetimeE);
            }
        }

        [TestMethod]
        public void OrLifetime_Or_OrLifetime_IsNewOrLifetime()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            using (var lifetimeD = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeE = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeF = lifetimeD.Or(lifetimeE))
            using (var lifetimeG = lifetimeC.Or(lifetimeF))
            {
                Assert.IsTrue(lifetimeG is OrLifetime);
                Assert.AreNotSame(lifetimeC, lifetimeG);
                Assert.AreNotSame(lifetimeF, lifetimeG);
            }
        }        

        #endregion

        #region [====== Starting and Expiring ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Or_Throws_IfCurrentLifetimeHasAlreadyStarted()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            {
                lifetimeA.Start();
                lifetimeA.Or(lifetimeB);
            }               
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Or_Throws_IfOtherLifetimeHasAlreadyStarted()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            {
                lifetimeB.Start();
                lifetimeA.Or(lifetimeB);
            }
        }

        [TestMethod]
        public void OrLifetime_StartsAggregatedLifetimes_WhenStarted()
        {
            using (var lifetimeA = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeB = TimerBasedLifetime.DefaultLifetime())
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            {
                lifetimeC.Start();

                Assert.IsTrue(lifetimeA.HasStarted);
                Assert.IsTrue(lifetimeB.HasStarted);
                Assert.IsTrue(lifetimeC.HasStarted);
            }
        }

        [TestMethod]
        public void OrLifetime_Expires_WhenOneOfTwoAggregatedLifetimesExpires()
        {
            using (var lifetimeA = new TimerBasedLifetime(TimeSpan.FromMilliseconds(1)))
            using (var lifetimeB = new TimerBasedLifetime(TimeSpan.FromDays(1)))
            using (var lifetimeC = lifetimeA.Or(lifetimeB))
            using (var waitHandle = new ManualResetEventSlim())
            {
                lifetimeC.Expired += (s, e) => waitHandle.Set();
                lifetimeC.Start();

                if (waitHandle.Wait(TimeSpan.FromMilliseconds(100)))
                {
                    Assert.IsTrue(lifetimeC.IsExpired);
                    return;
                }
                TimerBasedLifetimeTest.FailBecauseLifetimeDidNotExpire();
            }
        }

        [TestMethod]
        public void OrLifetime_Expires_WhenOneOfThreeAggregatedLifetimesExpires()
        {
            using (var lifetimeA = new TimerBasedLifetime(TimeSpan.FromMilliseconds(1)))
            using (var lifetimeB = new TimerBasedLifetime(TimeSpan.FromDays(1)))
            using (var lifetimeC = new TimerBasedLifetime(TimeSpan.FromSeconds(3)))
            using (var lifetimeD = lifetimeA.Or(lifetimeB).Or(lifetimeC))
            using (var waitHandle = new ManualResetEventSlim())
            {
                lifetimeD.Expired += (s, e) => waitHandle.Set();
                lifetimeD.Start();

                if (waitHandle.Wait(TimeSpan.FromMilliseconds(100)))
                {
                    Assert.IsTrue(lifetimeD.IsExpired);
                    return;
                }
                TimerBasedLifetimeTest.FailBecauseLifetimeDidNotExpire();
            }
        }

        #endregion
    }
}
