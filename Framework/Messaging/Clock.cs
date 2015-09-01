using System;

namespace ServiceComponents
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IClock" /> interface.
    /// </summary>
    public abstract class Clock : IClock
    {
        #region [====== Local Time ======]

        /// <inheritdoc />
        public TimeSpan LocalTime()
        {
            return LocalDateAndTime().TimeOfDay;
        }

        /// <inheritdoc />
        public DateTimeOffset LocalDate()
        {
            return StripTimeOfDay(LocalDateAndTime());
        }

        /// <inheritdoc />
        public virtual DateTimeOffset LocalDateAndTime()
        {
            return UtcDateAndTime().ToLocalTime();
        }

        #endregion

        #region [====== UTC Time ======]

        /// <inheritdoc />
        public TimeSpan UtcTime()
        {
            return UtcDateAndTime().TimeOfDay;
        }

        /// <inheritdoc />
        public DateTimeOffset UtcDate()
        {
            return StripTimeOfDay(UtcDateAndTime());
        }

        /// <inheritdoc />
        public abstract DateTimeOffset UtcDateAndTime();        

        #endregion

        /// <inheritdoc />
        public IClock Add(TimeSpan offset)
        {
            return ClockWithOffset.AddOffset(this, offset);
        }        

        /// <summary>
        /// Returns the clock associated to the current thread.
        /// </summary>
        public static IClock Current
        {
            get { return AsyncLocalClockContext.Instance.CurrentClock; }
        }

        /// <summary>
        /// Sets a clock that is globally visible as long as the scope is active.
        /// </summary>
        /// <param name="clock">The clock to use.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>        
        public static IDisposable OverrideStatic(IClock clock)
        {
            return new ClockScope(StaticClockContext.Instance, clock);
        }

        /// <summary>
        /// Sets a clock that is visible on the current thread as long as the scope is active.
        /// </summary>
        /// <param name="clock">The clock to use.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>
        public static IDisposable OverrideThreadStatic(IClock clock)
        {
            return new ClockScope(AsyncLocalClockContext.Instance, clock);
        }

        private static DateTimeOffset StripTimeOfDay(DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
        }
    }
}
