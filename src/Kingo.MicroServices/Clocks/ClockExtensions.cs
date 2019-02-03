using System;

namespace Kingo.Clocks
{
    /// <summary>
    /// Contains extension methods for the <see cref="IClock" /> interface.
    /// </summary>
    public static class ClockExtensions
    {
        /// <summary>
        /// Returns a clock that shows the time of the current clock, plus the given offset.
        /// </summary>
        /// <param name="clock">A clock.</param>
        /// <param name="offset">The offset to add to the time of the current clock.</param>
        /// <returns>A clock that shows the time of the current clock, plus the given offset.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="clock"/> is <c>null</c>.
        /// </exception>
        public static IClock Shift(this IClock clock, TimeSpan offset) =>
             ShiftedClock.Shift(clock, offset);
    }
}
