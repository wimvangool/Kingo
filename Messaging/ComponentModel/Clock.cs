namespace System.ComponentModel
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IClock" /> interface.
    /// </summary>
    public abstract class Clock : IClock
    {
        /// <inheritdoc />
        public TimeSpan UtcTime()
        {
            return LocalDateAndTime().ToUniversalTime().TimeOfDay;
        }

        /// <inheritdoc />
        public DateTimeOffset UtcDate()
        {
            return LocalDateAndTime().ToUniversalTime().Date;
        }

        /// <inheritdoc />
        public DateTimeOffset UtcDateAndTime()
        {
            return LocalDateAndTime().ToUniversalTime();
        }

        /// <inheritdoc />
        public TimeSpan LocalTime()
        {
            return LocalDateAndTime().TimeOfDay;
        }

        /// <inheritdoc />
        public DateTimeOffset LocalDate()
        {
            return LocalDateAndTime().Date;
        }

        /// <inheritdoc />
        public abstract DateTimeOffset LocalDateAndTime();

        /// <inheritdoc />
        public IClock Add(TimeSpan offset)
        {
            return ClockWithOffset.AddOffset(this, offset);
        }        

        /// <summary>
        /// Returns the clock that applies to the current thread.
        /// </summary>
        public static IClock Current
        {
            get { return ThreadStaticClockContext.Instance.CurrentClock; }
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
            return new ClockScope(ThreadStaticClockContext.Instance, clock);
        }
    }
}
