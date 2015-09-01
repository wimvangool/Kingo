using System;
using Kingo.BuildingBlocks.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks
{
    [TestClass]
    public sealed class ClockWithOffsetTest
    {
        [TestMethod]
        public void AddOffset_AddsSomeOffsetToSpecifiedClock()
        {
            var startTime = DateTimeOffset.UtcNow;
            var offset = new TimeSpan(6, 4, 2, 0);

            var stopwatch = new StopwatchClock(startTime);
            var clock = ClockWithOffset.AddOffset(stopwatch, offset);
            var dateAndTime = clock.UtcDateAndTime();

            Assert.AreEqual(startTime.Add(offset), dateAndTime);
        }

        [TestMethod]
        public void SubtractOffset_SubtractsSomeOffsetFromSpecifiedClock()
        {
            var startTime = DateTimeOffset.UtcNow;
            var offset = new TimeSpan(6, 4, 2, 0);

            var stopwatch = new StopwatchClock(startTime);
            var clock = ClockWithOffset.SubtractOffset(stopwatch, offset);
            var dateAndTime = clock.UtcDateAndTime();

            Assert.AreEqual(startTime.Subtract(offset), dateAndTime);
        }
    }
}
