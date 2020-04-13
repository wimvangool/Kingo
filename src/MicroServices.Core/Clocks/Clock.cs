using System;
using Kingo.Reflection;
using Kingo.Threading;

namespace Kingo.Clocks
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IClock" /> interface.
    /// </summary>
    public abstract class Clock : IClock
    {
        /// <summary>
        /// Returns the default clock of this system.
        /// </summary>
        public static readonly IClock SystemClock = new SystemClock();

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} - {LocalDateAndTime():O}";

        #region [====== Local Time ======]

        /// <inheritdoc />
        public TimeSpan LocalTime() =>
            LocalDateAndTime().TimeOfDay;

        /// <inheritdoc />
        public DateTimeOffset LocalDate() =>
            StripTimeOfDay(LocalDateAndTime());

        /// <inheritdoc />
        public virtual DateTimeOffset LocalDateAndTime() =>
            UtcDateAndTime().ToLocalTime();

        #endregion

        #region [====== UTC Time ======]

        /// <inheritdoc />
        public TimeSpan UtcTime() =>
            UtcDateAndTime().TimeOfDay;

        /// <inheritdoc />
        public DateTimeOffset UtcDate() =>
            StripTimeOfDay(UtcDateAndTime());

        /// <inheritdoc />
        public abstract DateTimeOffset UtcDateAndTime();

        #endregion

        private static DateTimeOffset StripTimeOfDay(DateTimeOffset value) =>
            new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
    }
}
