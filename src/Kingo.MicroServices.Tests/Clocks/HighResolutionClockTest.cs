using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Clocks
{
    [TestClass]
    public sealed class HighResolutionClockTest
    {
        private HighResolutionClock _clock;

        [TestInitialize]
        public void Setup()
        {
            _clock = HighResolutionClock.StartNew();            
        }

        [TestCleanup]
        public void TearDown()
        {
            _clock.Dispose();
        }
        
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void UtcDateAndTime_ReturnsIncrementedValue_EachTimeItIsCalled()
        {
            var values = new Queue<DateTimeOffset>();

            for (int index = 0; index < 10; index++)
            {
                WriteTimestampsTo(values, 100);

                Thread.Sleep(TimeSpan.FromMilliseconds(8));
            }
            AssertStrictlyAscending(values);
        }                

        private void WriteTimestampsTo(Queue<DateTimeOffset> values, int amount)
        {
            for (int index = 0; index < amount; index++)
            {
                values.Enqueue(_clock.UtcDateAndTime());
            }
        }

        private static void AssertStrictlyAscending(Queue<DateTimeOffset> values)
        {
            var previousValue = DateTimeOffset.MinValue;            

            while (values.Count > 0)
            {
                var nextValue = values.Dequeue();
                if (nextValue <= previousValue)
                {
                    Assert.Fail("Clock produced a timestamp ({0}) not greater than the previous one ({1}).", nextValue, previousValue);
                }
                previousValue = nextValue;
            }
        }
    }
}
