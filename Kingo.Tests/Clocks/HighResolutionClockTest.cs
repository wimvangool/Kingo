using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timer = System.Timers.Timer;

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

        //[TestMethod]
        public void WriteTimestampsToFile()
        {
            var timestamps = new Queue<DateTimeOffset>();

            using (var waitHandle = new ManualResetEventSlim())
            using (var timer = new Timer())            
            {
                timer.Interval = 600;
                timer.Elapsed += (s, e) => waitHandle.Set();
                timer.Start();

                do
                {
                    if (waitHandle.IsSet)
                    {
                        break;
                    }
                    timestamps.Enqueue(_clock.UtcDateAndTime());
                }
                while (true);
            }
            var fileName = string.Format(@"C:\temp\timestamps_{0}.txt", Guid.NewGuid());

            using (var fileWriter = new StreamWriter(fileName))
            {
                Debug.WriteLine("Writing {0} timestamps to file '{1}'...", timestamps.Count, fileName);

                while (timestamps.Count > 0)
                {
                    fileWriter.WriteLine(timestamps.Dequeue().Ticks);
                    fileWriter.Flush();
                }
            }
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
