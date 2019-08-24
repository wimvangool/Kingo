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

            var clock = new StopwatchClock(startTime);
            var shiftedClock = clock.Shift(offset);
            var dateAndTime = shiftedClock.UtcDateAndTime();

            Assert.AreEqual(startTime.Add(offset), dateAndTime);
        }

        [TestMethod]
        public void SubtractOffset_SubtractsSomeOffsetFromSpecifiedClock()
        {
            var startTime = DateTimeOffset.UtcNow;
            var offset = new TimeSpan(-6, -4, -2, 0);

            var clock = new StopwatchClock(startTime);
            var shiftedClock = clock.Shift(offset);
            var dateAndTime = shiftedClock.UtcDateAndTime();

            Assert.AreEqual(startTime.Add(offset), dateAndTime);
        }
    }
}
