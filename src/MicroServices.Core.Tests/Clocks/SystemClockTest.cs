using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Clocks
{
    [TestClass]
    public sealed class SystemClockTest
    {
        [TestMethod]
        public void LocalDate_ReturnsDateInLocalTime()
        {
            Assert.AreEqual(LocalTimeOffset(), Clock.SystemClock.LocalDate().Offset);
        }

        [TestMethod]
        public void LocalDateAndTime_ReturnsDateAndTimeInLocalTime()
        {
            Assert.AreEqual(LocalTimeOffset(), Clock.SystemClock.LocalDateAndTime().Offset);
        }

        [TestMethod]
        public void UtcDate_ReturnsDateInUniversalTime()
        {            
            Assert.AreEqual(TimeSpan.Zero, Clock.SystemClock.UtcDate().Offset);            
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsDateAndTimeInUniversalTime()
        {
            Assert.AreEqual(TimeSpan.Zero, Clock.SystemClock.UtcDateAndTime().Offset);
        }        

        private static TimeSpan LocalTimeOffset() =>
             DateTimeOffset.Now.Offset;
    }
}
