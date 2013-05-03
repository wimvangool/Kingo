using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents the clock that is running on the local machine.
    /// </summary>
    public sealed class MachineClock : IClock
    {
        /// <inheritdoc />
        public DateTime CurrentDate()
        {
            return DateTime.Today;
        }

        /// <inheritdoc />
        public DateTime CurrentDateTime()
        {
            return DateTime.Now;
        }

        /// <inheritdoc />
        public DateTime CurrentDateUtc()
        {
            return DateTime.UtcNow.Date;
        }

        /// <inheritdoc />
        public DateTime CurrentDateTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
