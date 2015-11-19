﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Clocks
{
    [TestClass]
    public sealed class ClockTest
    {
        [TestMethod]
        public void OverrideThreadStatic_WillSetClockForCurrentThread()
        {
            var startTime = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var stopwatch = new StopwatchClock(startTime);

            using (Clock.OverrideAsyncLocal(stopwatch))
            {
                var dateInsideScope = Clock.Current.UtcDate();

                Assert.AreEqual(2000, dateInsideScope.Year);
                Assert.AreEqual(1, dateInsideScope.Month);
                Assert.AreEqual(1, dateInsideScope.Day);
            }
            var dateOutsideScope = Clock.Current.UtcDate();

            Assert.IsTrue(dateOutsideScope.Year > 2000);            
        }
    }
}