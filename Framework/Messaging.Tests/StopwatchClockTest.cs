using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
    [TestClass]
    public sealed class StopwatchClockTest
    {
        [TestMethod]
        public void LocalTime_ReturnsTimeInLocalTime()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, 8, 11, 36);

            Assert.AreEqual(new TimeSpan(8, 11, 36), stopwatch.LocalTime());
        }

        [TestMethod]
        public void LocalDate_ReturnsDateInLocalTime()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, 6, 13, 57);
            var localDate = stopwatch.LocalDate();
            
            Assert.AreEqual(2000, localDate.Year);
            Assert.AreEqual(1, localDate.Month);
            Assert.AreEqual(1, localDate.Day);
            Assert.AreEqual(TimeSpan.Zero, localDate.TimeOfDay);            
        }

        [TestMethod]
        public void LocalDateAndTime_ReturnsDateAndTimeInLocalTime()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, 4, 20, 23);
            var localDateAndTime = stopwatch.LocalDateAndTime();
            
            Assert.AreEqual(2000, localDateAndTime.Year);
            Assert.AreEqual(1, localDateAndTime.Month);
            Assert.AreEqual(1, localDateAndTime.Day);
            Assert.AreEqual(4, localDateAndTime.Hour);
            Assert.AreEqual(20, localDateAndTime.Minute);
            Assert.AreEqual(23, localDateAndTime.Second);
        }

        [TestMethod]
        public void UtcTime_ReturnsTimeInUniversalTime()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, 8, 44, 2, DateTimeKind.Utc);

            Assert.AreEqual(new TimeSpan(8, 44, 2), stopwatch.UtcTime());
        }

        [TestMethod]
        public void UtcDate_ReturnsDateInUniversalTime()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, 2, 55, 12, DateTimeKind.Utc);
            var utcDate = stopwatch.UtcDate();
            
            Assert.AreEqual(2000, utcDate.Year);
            Assert.AreEqual(1, utcDate.Month);
            Assert.AreEqual(1, utcDate.Day);
            Assert.AreEqual(TimeSpan.Zero, utcDate.TimeOfDay);
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsDateAndTimeInUniversalTime()
        {
            var stopwatch = new StopwatchClock(2000, 1, 1, 4, 20, 23, DateTimeKind.Utc);
            var utcDateAndTime = stopwatch.UtcDateAndTime();
            
            Assert.AreEqual(2000, utcDateAndTime.Year);
            Assert.AreEqual(1, utcDateAndTime.Month);
            Assert.AreEqual(1, utcDateAndTime.Day);
            Assert.AreEqual(4, utcDateAndTime.Hour);
            Assert.AreEqual(20, utcDateAndTime.Minute);
            Assert.AreEqual(23, utcDateAndTime.Second);
        }

        [TestMethod]
        public void ElapsedTime_StartsAtZero()
        {
            var stopwatch = new StopwatchClock(SystemClock.Instance.LocalDateAndTime());

            Assert.AreEqual(TimeSpan.Zero, stopwatch.ElapsedTime);
        }

        [TestMethod]
        public void Start_WillStartTheClock()
        {
            var stopwatch = new StopwatchClock(2012, 1, 1);

            stopwatch.Start();

            try
            {
                Thread.Sleep(2);

                Assert.IsTrue(stopwatch.ElapsedTime >= TimeSpan.FromMilliseconds(1.0));
                Assert.IsTrue(stopwatch.LocalDateAndTime() > new DateTime(2012, 1, 1));
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        [TestMethod]
        public void Stop_WillStopTheClock()
        {
            var stopwatch = new StopwatchClock(1900, 1, 1);

            stopwatch.Start();

            Thread.Sleep(2);

            stopwatch.Stop();

            var elapsedTimeBefore = stopwatch.ElapsedTime;

            Thread.Sleep(2);

            var elapsedTimeAfter = stopwatch.ElapsedTime;

            Assert.AreEqual(elapsedTimeBefore, elapsedTimeAfter);
        }
    }
}
