using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class ClockWithOffsetTest
    {
        [TestMethod]
        public void AddOffset_AddsSomeOffsetToSpecifiedClock()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, DateTimeKind.Local);
            var clock = ClockWithOffset.AddOffset(stopwatch, new TimeSpan(6, 4, 2, 0));
            var dateAndTime = clock.LocalDateAndTime();

            Assert.AreEqual(2000, dateAndTime.Year);
            Assert.AreEqual(1, dateAndTime.Month);
            Assert.AreEqual(7, dateAndTime.Day);
            Assert.AreEqual(4, dateAndTime.Hour);
            Assert.AreEqual(2, dateAndTime.Minute);
        }

        [TestMethod]
        public void SubtractOffset_SubtractsSomeOffsetFromSpecifiedClock()
        {
            var stopwatch = new StopwatchClock(2000, 1, 8, DateTimeKind.Local);
            var clock = ClockWithOffset.SubtractOffset(stopwatch, new TimeSpan(6, 4, 2, 0));
            var dateAndTime = clock.LocalDateAndTime();

            Assert.AreEqual(2000, dateAndTime.Year);
            Assert.AreEqual(1, dateAndTime.Month);
            Assert.AreEqual(1, dateAndTime.Day);
            Assert.AreEqual(19, dateAndTime.Hour);
            Assert.AreEqual(58, dateAndTime.Minute);
        }
    }
}
