using System;

namespace Kingo.Clocks
{
    /// <summary>
    /// Contains extension methods for the <see cref="IClock" /> interface.
    /// </summary>
    public static class ClockExtensions
    {
        /// <summary>
        /// Returns a clock that will be set to the specified <paramref name="value" /> but will otherwise
        /// be completely synchronized with the specified <paramref name="referenceClock" /> regarding the
        /// progress of time.
        /// </summary>
        /// <param name="referenceClock">A clock.</param>
        /// <param name="value">The value to which the new clock will be set.</param>
        /// <returns>A clock that will be set to the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="referenceClock"/> is <c>null</c>.
        /// </exception>
        public static IClock SetToSpecificTime(this IClock referenceClock, DateTimeOffset value) =>
            new SpecificTimeClock(referenceClock, value);

        /// <summary>
        /// Returns a clock that shows the time of the specified <paramref name="referenceClock"/>, plus or minus the given offset.
        /// </summary>
        /// <param name="referenceClock">A clock.</param>
        /// <param name="offset">The offset to add to the time of the current clock.</param>
        /// <returns>A clock that shows the time of the current clock, plus the given offset.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="referenceClock"/> is <c>null</c>.
        /// </exception>
        public static IClock Shift(this IClock referenceClock, TimeSpan offset) =>
             new ShiftedClock(referenceClock, offset);
    }
}
