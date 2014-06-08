using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YellowFlare.MessageProcessing.Clocks
{
    [TestClass]
    public sealed class ClockTest
    {
        [TestMethod]
        public void OverrideThreadStatic_WillSetClockForCurrentThread()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, DateTimeKind.Local);

            using (Clock.OverrideThreadStatic(stopwatch))
            {
                var dateInsideScope = Clock.Current.LocalDate();

                Assert.AreEqual(2000, dateInsideScope.Year);
                Assert.AreEqual(1, dateInsideScope.Month);
                Assert.AreEqual(1, dateInsideScope.Day);
            }
            var dateOutsideScope = Clock.Current.LocalDate();

            Assert.IsTrue(dateOutsideScope.Year != 2000);            
        }
    }
}
