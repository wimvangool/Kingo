using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Clocks
{
    [TestClass]
    public sealed class StopwatchClockTest
    {
        [TestMethod]
        public void LocalTime_ReturnsTimeInLocalTime()
        {
            var startTime = DateTimeOffset.Now;
            var stopwatch = new StopwatchClock(startTime);
            var localTime = startTime.TimeOfDay;

            Assert.AreEqual(localTime, stopwatch.LocalTime());
        }

        [TestMethod]
        public void LocalDate_ReturnsDateInLocalTime()
        {
            var startTime = DateTimeOffset.Now;
            var stopwatch = new StopwatchClock(startTime);
            var localDate = stopwatch.LocalDate();
            
            Assert.AreEqual(startTime.Year, localDate.Year);
            Assert.AreEqual(startTime.Month, localDate.Month);
            Assert.AreEqual(startTime.Day, localDate.Day);
            Assert.AreEqual(TimeSpan.Zero, localDate.TimeOfDay);            
        }

        [TestMethod]
        public void LocalDateAndTime_ReturnsDateAndTimeInLocalTime()
        {
            var startTime = DateTimeOffset.Now;
            var stopwatch = new StopwatchClock(startTime);
            var localDateAndTime = stopwatch.LocalDateAndTime();
            
            Assert.AreEqual(startTime.Year, localDateAndTime.Year);
            Assert.AreEqual(startTime.Month, localDateAndTime.Month);
            Assert.AreEqual(startTime.Day, localDateAndTime.Day);
            Assert.AreEqual(startTime.Hour, localDateAndTime.Hour);
            Assert.AreEqual(startTime.Minute, localDateAndTime.Minute);
            Assert.AreEqual(startTime.Second, localDateAndTime.Second);
        }

        [TestMethod]
        public void UtcTime_ReturnsTimeInUniversalTime()
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = new StopwatchClock(startTime);
            var utcTime = startTime.TimeOfDay;

            Assert.AreEqual(utcTime, stopwatch.UtcTime());
        }

        [TestMethod]
        public void UtcDate_ReturnsDateInUniversalTime()
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = new StopwatchClock(startTime);
            var utcDate = stopwatch.UtcDate();
            
            Assert.AreEqual(startTime.Year, utcDate.Year);
            Assert.AreEqual(startTime.Month, utcDate.Month);
            Assert.AreEqual(startTime.Day, utcDate.Day);
            Assert.AreEqual(TimeSpan.Zero, utcDate.TimeOfDay);
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsDateAndTimeInUniversalTime()
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = new StopwatchClock(startTime);
            var utcDateAndTime = stopwatch.UtcDateAndTime();
            
            Assert.AreEqual(startTime.Year, utcDateAndTime.Year);
            Assert.AreEqual(startTime.Month, utcDateAndTime.Month);
            Assert.AreEqual(startTime.Day, utcDateAndTime.Day);
            Assert.AreEqual(startTime.Hour, utcDateAndTime.Hour);
            Assert.AreEqual(startTime.Minute, utcDateAndTime.Minute);
            Assert.AreEqual(startTime.Second, utcDateAndTime.Second);
        }

        [TestMethod]
        public void ElapsedTime_StartsAtZero()
        {
            var stopwatch = new StopwatchClock(DateTimeOffset.UtcNow);

            Assert.AreEqual(TimeSpan.Zero, stopwatch.ElapsedTime);
        }

        [TestMethod]
        public void Start_WillStartTheClock()
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = new StopwatchClock(DateTimeOffset.UtcNow);

            stopwatch.Start();

            try
            {
                Thread.Sleep(2);

                Assert.IsTrue(stopwatch.ElapsedTime >= TimeSpan.FromMilliseconds(1.0));
                Assert.IsTrue(stopwatch.UtcDateAndTime() > startTime);
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        [TestMethod]
        public void Stop_WillStopTheClock()
        {
            var stopwatch = new StopwatchClock(DateTimeOffset.UtcNow);
            
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
