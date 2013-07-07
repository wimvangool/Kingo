using System;
using System.Diagnostics;

namespace YellowFlare.MessageProcessing.Clocks
{
    public sealed class StopwatchClock : Clock
    {
        private readonly DateTime _localDateAndTimeStart;
        private readonly Stopwatch _stopwatch;

        public StopwatchClock(int year, int month, int day)
            : this(new DateTime(year, month, day)) { }

        public StopwatchClock(int year, int month, int day, DateTimeKind kind)
            : this(new DateTime(year, month, day, 0, 0, 0, kind)) { }

        public StopwatchClock(int year, int month, int day, int hour, int minute, int second)
            : this(new DateTime(year, month, day, hour, minute, second)) { }

        public StopwatchClock(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
            : this(new DateTime(year, month, day, hour, minute, second, kind)) { }

        public StopwatchClock(DateTime startTime)
        {
            _localDateAndTimeStart = InitializeStartTime(startTime);
            _stopwatch = new Stopwatch();
        }        

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public TimeSpan ElapsedTime
        {
            get { return TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds); }
        }

        public override DateTime LocalDateAndTime()
        {
            return _localDateAndTimeStart.AddMilliseconds(_stopwatch.ElapsedMilliseconds);
        }

        private static DateTime InitializeStartTime(DateTime startTime)
        {
            if (startTime.Kind == DateTimeKind.Utc)
            {
                return startTime.ToLocalTime();
            }
            if (startTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(startTime, DateTimeKind.Local);
            }
            return startTime;
        }
    }
}
