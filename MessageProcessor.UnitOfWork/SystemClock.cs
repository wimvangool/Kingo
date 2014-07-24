using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents the clock that is running on the local machine or system.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        private SystemClock() {}

        /// <inheritdoc />
        public TimeSpan UtcTime()
        {
            return DateTime.UtcNow.TimeOfDay;
        }

        /// <inheritdoc />
        public DateTime UtcDate()
        {
            return DateTime.UtcNow.Date;
        }

        /// <inheritdoc />
        public DateTime UtcDateAndTime()
        {
            return DateTime.UtcNow;
        }

        /// <inheritdoc />
        public TimeSpan LocalTime()
        {
            return DateTime.Now.TimeOfDay;
        }

        /// <inheritdoc />
        public DateTime LocalDate()
        {
            return DateTime.Today;
        }

        /// <inheritdoc />
        public DateTime LocalDateAndTime()
        {
            return DateTime.Now;
        }

        /// <inheritdoc />
        public IClock Add(TimeSpan offset)
        {
            return ClockWithOffset.AddOffset(this, offset);
        }

        /// <inheritdoc />
        public IClock Subtract(TimeSpan offset)
        {
            return ClockWithOffset.SubtractOffset(this, offset);
        }

        /// <summary>
        /// The system-clock of the current system.
        /// </summary>
        public static readonly SystemClock Instance = new SystemClock();        
    }
}
