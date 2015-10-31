using System;
using System.Runtime.Remoting.Messaging;

namespace Kingo.BuildingBlocks.Clocks
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

        /// <summary>
        /// Returns the default clock of this system.
        /// </summary>
        public static readonly IClock Default = new DefaultClock(); 

        /// <summary>
        /// Returns the clock associated to the current thread.
        /// </summary>
        public static IClock Current
        {
            get { return AsyncLocalClockContext.Instance.CurrentClock; }
        }

        /// <summary>
        /// Sets a clock that is globally accessible through <see cref="Clock.Current" /> as long as the scope is active.
        /// Note that the specified <paramref name="clock"/> will only be accessible through <see cref="Clock.Current"/> as long as
        /// <see cref="OverrideAsyncLocal(IClock)" /> has not been called for the current <see cref="LogicalCallContext" />.
        /// </summary>
        /// <param name="clock">The clock to use.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>        
        public static IDisposable Override(IClock clock)
        {
            return new ClockScope(StaticClockContext.Instance, clock);
        }

        /// <summary>
        /// Sets a clock that is accessible through <see cref="Clock.Current" /> on the current <see cref="LogicalCallContext" />
        /// as long as the scope is active.
        /// </summary>
        /// <param name="clock">The clock to use.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>
        public static IDisposable OverrideAsyncLocal(IClock clock)
        {
            return new ClockScope(AsyncLocalClockContext.Instance, clock);
        }

        private static DateTimeOffset StripTimeOfDay(DateTimeOffset value)
        {
            return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
        }
    }
}
