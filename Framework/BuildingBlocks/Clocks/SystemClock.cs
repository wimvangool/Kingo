using System;

namespace Kingo.BuildingBlocks.Clocks
{
    /// <summary>
    /// Represents the clock that is running on the local machine or system.
    /// </summary>
    public sealed class SystemClock : Clock
    {
        private SystemClock() {}

        /// <inheritdoc />
        public override DateTimeOffset LocalDateAndTime()
        {
            return DateTimeOffset.Now;
        }

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime()
        {
            return DateTimeOffset.UtcNow;
        }        

        /// <summary>
        /// The system-clock of the current system.
        /// </summary>
        public static readonly SystemClock Instance = new SystemClock();               
    }
}
