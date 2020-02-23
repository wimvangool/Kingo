using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IGivenState" />.
    /// </summary>
    public static class GivenStateExtensions
    {
        /// <summary>
        /// Sets the current date and time to a specific value before or between operations.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="year">Year of the time to set.</param>
        /// <param name="month">Month of the time to set.</param>
        /// <param name="day">Day of the time to set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// One of the time-components is not valid - or - the time is a time in the past relative to the current timeline.
        /// </exception>
        public static void TimeIs(this IGivenState state, int year, int month, int day) =>
            NotNull(state).TimeIs(year, month, day, 0, 0, 0);

        /// <summary>
        /// Sets the current date and time to a specific value before or between operations.
        /// </summary>
        /// <param name="state">State of the test-engine.</param>
        /// <param name="year">Year of the time to set.</param>
        /// <param name="month">Month of the time to set.</param>
        /// <param name="day">Day of the time to set.</param>
        /// <param name="hour">Hour of the time to set.</param>
        /// <param name="minute">Minute of the time to set.</param>
        /// <param name="second">Second of the time to set.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// One of the time-components is not valid - or - the time is a time in the past relative to the current timeline.
        /// </exception>
        public static void TimeIs(this IGivenState state, int year, int month, int day, int hour, int minute, int second) =>
            NotNull(state).TimeIs(new DateTime(year, month, day, hour, minute, second));

        private static IGivenState NotNull(IGivenState state) =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
