using System;
using System.Diagnostics;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// This clock represents a stopwatch that starts at a specified date and/or time and can be started and stopped.
    /// </summary>
    public sealed class StopwatchClock : Clock
    {
        private readonly DateTime _localDateAndTimeStart;
        private readonly Stopwatch _stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchClock" /> class.
        /// </summary>
        /// <param name="year">The year of the initial date.</param>
        /// <param name="month">The month of the initial date.</param>
        /// <param name="day">The day of the initial date.</param>
        /// <exception cref="ArgumentException">
        /// The values do not represent a valid date.
        /// </exception>
        public StopwatchClock(int year, int month, int day)
            : this(new DateTime(year, month, day)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchClock" /> class.
        /// </summary>
        /// <param name="year">The year of the initial date.</param>
        /// <param name="month">The month of the initial date.</param>
        /// <param name="day">The day of the initial date.</param>
        /// <param name="kind">Indicates how to interpret the values.</param>
        /// <exception cref="ArgumentException">
        /// The values do not represent a valid date.
        /// </exception>
        public StopwatchClock(int year, int month, int day, DateTimeKind kind)
            : this(new DateTime(year, month, day, 0, 0, 0, kind)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchClock" /> class.
        /// </summary>
        /// <param name="year">The year of the initial date.</param>
        /// <param name="month">The month of the initial date.</param>
        /// <param name="day">The day of the initial date.</param>
        /// <param name="hour">The hour of the initial time.</param>
        /// <param name="minute">The minute of the initial time.</param>
        /// <param name="second">The second of the initial time.</param>
        /// <exception cref="ArgumentException">
        /// The values do not represent a valid date.
        /// </exception>
        public StopwatchClock(int year, int month, int day, int hour, int minute, int second)
            : this(new DateTime(year, month, day, hour, minute, second)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchClock" /> class.
        /// </summary>
        /// <param name="year">The year of the initial date.</param>
        /// <param name="month">The month of the initial date.</param>
        /// <param name="day">The day of the initial date.</param>
        /// <param name="hour">The hour of the initial time.</param>
        /// <param name="minute">The minute of the initial time.</param>
        /// <param name="second">The second of the initial time.</param>
        /// <param name="kind">Indicates how to interpret the values.</param>
        /// <exception cref="ArgumentException">
        /// The values do not represent a valid date.
        /// </exception>
        public StopwatchClock(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
            : this(new DateTime(year, month, day, hour, minute, second, kind)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchClock" /> class.
        /// </summary>
        /// <param name="startTime">The initial date and time.</param>
        public StopwatchClock(DateTime startTime)
        {
            _localDateAndTimeStart = InitializeStartTime(startTime);
            _stopwatch = new Stopwatch();
        }        

        /// <summary>
        /// Starts the clock.
        /// </summary>
        public void Start()
        {
            _stopwatch.Start();
        }

        /// <summary>
        /// Stops the clock.
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
        }

        /// <summary>
        /// Returns the total amount of time the stopwatch has been running.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return TimeSpan.FromMilliseconds(_stopwatch.ElapsedMilliseconds); }
        }

        /// <inheritdoc />
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
