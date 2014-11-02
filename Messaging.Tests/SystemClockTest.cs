using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class SystemClockTest
    {
        [TestMethod]
        public void LocalDate_ReturnsDateInLocalTime()
        {
            Assert.AreEqual(DateTimeKind.Local, SystemClock.Instance.LocalDate().Kind);
        }

        [TestMethod]
        public void LocalDateAndTime_ReturnsDateAndTimeInLocalTime()
        {
            Assert.AreEqual(DateTimeKind.Local, SystemClock.Instance.LocalDateAndTime().Kind);
        }

        [TestMethod]
        public void UtcDate_ReturnsDateInUniversalTime()
        {
            Assert.AreEqual(DateTimeKind.Utc, SystemClock.Instance.UtcDate().Kind);
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsDateAndTimeInUniversalTime()
        {
            Assert.AreEqual(DateTimeKind.Utc, SystemClock.Instance.UtcDateAndTime().Kind);
        }
    }
}
