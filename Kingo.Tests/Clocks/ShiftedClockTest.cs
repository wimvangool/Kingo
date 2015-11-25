using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Clocks
{
    [TestClass]
    public sealed class ShiftedClockTest
    {
        [TestMethod]
        public void AddOffset_AddsSomeOffsetToSpecifiedClock()
        {
            var startTime = DateTimeOffset.UtcNow;
            var offset = new TimeSpan(6, 4, 2, 0);

            var stopwatch = new StopwatchClock(startTime);
            var clock = ShiftedClock.Shift(stopwatch, offset);
            var dateAndTime = clock.UtcDateAndTime();

            Assert.AreEqual(startTime.Add(offset), dateAndTime);
        }

        [TestMethod]
        public void SubtractOffset_SubtractsSomeOffsetFromSpecifiedClock()
        {
            var startTime = DateTimeOffset.UtcNow;
            var offset = new TimeSpan(-6, -4, -2, 0);

            var stopwatch = new StopwatchClock(startTime);
            var clock = ShiftedClock.Shift(stopwatch, offset);
            var dateAndTime = clock.UtcDateAndTime();

            Assert.AreEqual(startTime.Add(offset), dateAndTime);
        }
    }
}
