namespace System.ComponentModel
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
        public DateTimeOffset UtcDate()
        {
            return StripTimeOfDay(DateTimeOffset.UtcNow);
        }

        /// <inheritdoc />
        public DateTimeOffset UtcDateAndTime()
        {
            return DateTimeOffset.UtcNow;
        }

        /// <inheritdoc />
        public TimeSpan LocalTime()
        {
            return DateTimeOffset.Now.TimeOfDay;
        }

        /// <inheritdoc />
        public DateTimeOffset LocalDate()
        {
            return StripTimeOfDay(DateTimeOffset.Now);
        }

        /// <inheritdoc />
        public DateTimeOffset LocalDateAndTime()
        {
            return DateTimeOffset.Now;
        }

        /// <inheritdoc />
        public IClock Add(TimeSpan offset)
        {
            return ClockWithOffset.AddOffset(this, offset);
        }        

        /// <summary>
        /// The system-clock of the current system.
        /// </summary>
        public static readonly SystemClock Instance = new SystemClock();
        
        private static DateTimeOffset StripTimeOfDay(DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
        }
    }
}
