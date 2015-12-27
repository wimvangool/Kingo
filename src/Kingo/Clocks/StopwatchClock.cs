using System;
using System.Diagnostics;

namespace Kingo.Clocks
{
    /// <summary>
    /// This clock represents a stopwatch that starts at a specified date and/or time and can be started and stopped.
    /// </summary>
    public sealed class StopwatchClock : Clock
    {
        private readonly DateTimeOffset _startTime;
        private readonly Stopwatch _stopwatch;        

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchClock" /> class.
        /// </summary>
        /// <param name="startTime">The initial date and time.</param>
        public StopwatchClock(DateTimeOffset startTime)
        {
            _startTime = startTime.ToUniversalTime();
            _stopwatch = new Stopwatch();
        }        

        /// <summary>
        /// Indicates whether or not this clock is running.
        /// </summary>
        public bool IsRunning
        {
            get { return _stopwatch.IsRunning; }
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
            get { return _stopwatch.Elapsed; }
        }        

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime()
        {
            lock (_stopwatch)
            {
                return _startTime.Add(_stopwatch.Elapsed);
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="StopwatchClock" /> that is started immediately.
        /// </summary>        
        /// <returns>A new <see cref="StopwatchClock" />.</returns>
        public static StopwatchClock StartNew()
        {
            return StartNew(DateTimeOffset.UtcNow);
        }

        /// <summary>
        /// Creates and returns a new <see cref="StopwatchClock" /> that is started immediately.
        /// </summary>    
        /// <param name="startTime">The initial date and time.</param>    
        /// <returns>A new <see cref="StopwatchClock" />.</returns>
        public static StopwatchClock StartNew(DateTimeOffset startTime)
        {
            var clock = new StopwatchClock(startTime);
            clock.Start();
            return clock;
        }
    }
}
