using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public class ScenarioClockTest
    {
        private readonly DateTime _dateTimeOffSet;
        private readonly DateTime _dateTimeOffSetUtc;
        private ScenarioClock _clock;
        
        public ScenarioClockTest()
        {
            _dateTimeOffSet = new DateTime(2000, 1, 1, 8, 0, 0);
            _dateTimeOffSetUtc = _dateTimeOffSet.ToUniversalTime();
        }

        [TestInitialize]
        public void Setup()
        {
            _clock = new ScenarioClock(_dateTimeOffSet);
            _clock.Start();            
        }

        [TestCleanup]
        public void Teardown()
        {
            _clock.Stop();    
        }

        [TestMethod]
        public void CurrentDate_ReturnsTheCurrentDate()
        {
            var date = _clock.CurrentDate();
            var time = date.TimeOfDay;

            Assert.AreEqual(_dateTimeOffSet.Year, date.Year);
            Assert.AreEqual(_dateTimeOffSet.Month, date.Month);
            Assert.AreEqual(_dateTimeOffSet.Day, date.Day);
            Assert.AreEqual(TimeSpan.Zero, time);
        }

        [TestMethod]
        public void CurrentDateTime_ReturnsTheCurrentDateAndTime()
        {
            var date = _clock.CurrentDateTime();
            var time = date.TimeOfDay;

            Assert.AreEqual(_dateTimeOffSet.Year, date.Year);
            Assert.AreEqual(_dateTimeOffSet.Month, date.Month);
            Assert.AreEqual(_dateTimeOffSet.Day, date.Day);
            Assert.AreNotEqual(TimeSpan.Zero, time);
        }

        [TestMethod]
        public void CurrentDateUtc_ReturnsTheCurrentDateInUniversalTime()
        {
            var date = _clock.CurrentDateUtc();
            var time = date.TimeOfDay;

            Assert.AreEqual(_dateTimeOffSetUtc.Year, date.Year);
            Assert.AreEqual(_dateTimeOffSetUtc.Month, date.Month);
            Assert.AreEqual(_dateTimeOffSetUtc.Day, date.Day);
            Assert.AreEqual(TimeSpan.Zero, time);
        }

        [TestMethod]
        public void CurrentDateTimeUtc_ReturnsTheCurrentDateAndTimeInUniversalTime()
        {
            var date = _clock.CurrentDateTimeUtc();
            var time = date.TimeOfDay;

            Assert.AreEqual(_dateTimeOffSetUtc.Year, date.Year);
            Assert.AreEqual(_dateTimeOffSetUtc.Month, date.Month);
            Assert.AreEqual(_dateTimeOffSetUtc.Day, date.Day);
            Assert.AreNotEqual(TimeSpan.Zero, time);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestAt_Throw_IfIndexIsNegative()
        {
            _clock.RequestAt(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestAt_Throws_IfNoRequestWasMadeAtTheSpecifiedIndex()
        {
            _clock.RequestAt(0);
        }

        [TestMethod]
        public void RequestAt_ReturnsRequestedDate_IfRequestWasMadeAtTheSpecifiedIndex()
        {
            Assert.AreEqual(_clock.CurrentDate(), _clock.RequestAt(0).Date);           
        }

        [TestMethod]
        public void RequestAt_ReturnsRequestedDateTime_IfRequestWasMadeAtTheSpecifiedIndex()
        {
            Assert.AreEqual(_clock.CurrentDateTime(), _clock.RequestAt(0));  
        }

        [TestMethod]
        public void RequestAt_ReturnsRequestedDateUtc_IfRequestWasMadeAtTheSpecifiedIndex()
        {
            Assert.AreEqual(_clock.CurrentDateUtc(), _clock.RequestAt(0).ToUniversalTime().Date);  
        }

        [TestMethod]
        public void RequestAt_ReturnsRequestedDateTimeUtc_IfRequestWasMadeAtTheSpecifiedIndex()
        {
            Assert.AreEqual(_clock.CurrentDateTimeUtc(), _clock.RequestAt(0).ToUniversalTime());  
        }
    }
}
