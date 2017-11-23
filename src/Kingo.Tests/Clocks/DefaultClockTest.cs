using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Clocks
{
    [TestClass]
    public sealed class DefaultClockTest
    {
        [TestMethod]
        public void LocalDate_ReturnsDateInLocalTime()
        {
            Assert.AreEqual(LocalTimeOffset(), Clock.Default.LocalDate().Offset);
        }

        [TestMethod]
        public void LocalDateAndTime_ReturnsDateAndTimeInLocalTime()
        {
            Assert.AreEqual(LocalTimeOffset(), Clock.Default.LocalDateAndTime().Offset);
        }

        [TestMethod]
        public void UtcDate_ReturnsDateInUniversalTime()
        {            
            Assert.AreEqual(TimeSpan.Zero, Clock.Default.UtcDate().Offset);            
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsDateAndTimeInUniversalTime()
        {
            Assert.AreEqual(TimeSpan.Zero, Clock.Default.UtcDateAndTime().Offset);
        }        

        private static TimeSpan LocalTimeOffset() => DateTimeOffset.Now.Offset;
    }
}
