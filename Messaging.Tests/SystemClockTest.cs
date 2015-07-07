using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Syztem
{
    [TestClass]
    public sealed class SystemClockTest
    {
        [TestMethod]
        public void LocalDate_ReturnsDateInLocalTime()
        {
            Assert.AreEqual(LocalTimeOffset(), SystemClock.Instance.LocalDate().Offset);
        }

        [TestMethod]
        public void LocalDateAndTime_ReturnsDateAndTimeInLocalTime()
        {
            Assert.AreEqual(LocalTimeOffset(), SystemClock.Instance.LocalDateAndTime().Offset);
        }

        [TestMethod]
        public void UtcDate_ReturnsDateInUniversalTime()
        {            
            Assert.AreEqual(TimeSpan.Zero, SystemClock.Instance.UtcDate().Offset);            
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsDateAndTimeInUniversalTime()
        {
            Assert.AreEqual(TimeSpan.Zero, SystemClock.Instance.UtcDateAndTime().Offset);
        }        

        private static TimeSpan LocalTimeOffset()
        {
            return DateTimeOffset.Now.Offset;
        }
    }
}
