using System;
using System.Runtime.Remoting.Messaging;
using Kingo.Threading;

namespace Kingo.Clocks
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IClock" /> interface.
    /// </summary>
    public abstract class Clock : IClock
    {
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

        #region [====== Current ======]

        /// <summary>
        /// Returns the default clock of this system.
        /// </summary>
        public static readonly IClock Default = new DefaultClock();

        private static readonly Context<IClock> _Context = new Context<IClock>(Default);              

        /// <summary>
        /// Returns the clock associated to the current thread.
        /// </summary>
        public static IClock Current =>
            _Context.Current;

        /// <summary>
        /// Sets the current clock that is accessible by the current thread through <see cref="Current" />
        /// only as long as the scope is active.
        /// </summary>
        /// <param name="utcDateAndTimeFactory">The delegate that is used to obtain the UTC date and time.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="utcDateAndTimeFactory"/> is <c>null</c>.
        /// </exception>                       
        public static IDisposable OverrideThreadLocal(Func<DateTimeOffset> utcDateAndTimeFactory) =>
            _Context.OverrideThreadLocal(new DelegateClock(utcDateAndTimeFactory));

        /// <summary>
        /// Sets the current clock that is accessible by the current thread through <see cref="Current" />
        /// only as long as the scope is active.
        /// </summary>
        /// <param name="clock">The clock to set.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>        
        public static IDisposable OverrideThreadLocal(IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }
            return _Context.OverrideThreadLocal(clock);
        }

        /// <summary>
        /// Sets the current value that is accessible by all threads that share the same <see cref="LogicalCallContext" />
        /// through <see cref="Current" /> as long as the scope is active.
        /// </summary>
        /// <param name="utcDateAndTimeFactory">The delegate that is used to obtain the UTC date and time.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="utcDateAndTimeFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The call is made inside a thread local scope.
        /// </exception>
        public static IDisposable OverrideAsyncLocal(Func<DateTimeOffset> utcDateAndTimeFactory) =>
            _Context.OverrideAsyncLocal(new DelegateClock(utcDateAndTimeFactory));

        /// <summary>
        /// Sets the current value that is accessible by all threads that share the same <see cref="LogicalCallContext" />
        /// through <see cref="Current" /> as long as the scope is active.
        /// </summary>
        /// <param name="clock">The clock to set.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The call is made inside a thread local scope.
        /// </exception>
        public static IDisposable OverrideAsyncLocal(IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }
            return _Context.OverrideAsyncLocal(clock);
        }

        /// <summary>
        /// Sets the current value that is accessible by all threads
        /// through <see cref="Current" /> as long as the scope is active.
        /// </summary>
        /// <param name="utcDateAndTimeFactory">The delegate that is used to obtain the UTC date and time.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="utcDateAndTimeFactory"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="InvalidOperationException">
        /// The call is made inside an async local or thread local scope.
        /// </exception>      
        public static IDisposable Override(Func<DateTimeOffset> utcDateAndTimeFactory) =>
            _Context.Override(new DelegateClock(utcDateAndTimeFactory));

        /// <summary>
        /// Sets the current value that is accessible by all threads
        /// through <see cref="Current" /> as long as the scope is active.
        /// </summary>
        /// <param name="clock">The clock to set.</param>
        /// <returns>The scope that is to be disposed when ended.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>  
        /// <exception cref="InvalidOperationException">
        /// The call is made inside an async local or thread local scope.
        /// </exception>      
        public static IDisposable Override(IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }
            return _Context.Override(clock);
        }           

        #endregion
    }
}
