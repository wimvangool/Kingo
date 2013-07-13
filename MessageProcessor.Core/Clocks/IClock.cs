using System;

namespace YellowFlare.MessageProcessing.Clocks
{
    /// <summary>
    /// When implemented by a class, represents a service that provides the current date and time.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Returns the time of the day in UTC-time.
        /// </summary>
        /// <returns>Time of the day in UTC-time.</returns>
        TimeSpan UtcTime();

        /// <summary>
        /// Returns the date in UTC-time.
        /// </summary>
        /// <returns>Date in UTC-time.</returns>
        DateTime UtcDate();

        /// <summary>
        /// Returns the date and time in UTC-time.
        /// </summary>
        /// <returns>Date and time in UTC-time.</returns>
        DateTime UtcDateAndTime();

        /// <summary>
        /// Returns the time of the day in local time.
        /// </summary>
        /// <returns>The time of the day in local time.</returns>
        TimeSpan LocalTime();

        /// <summary>
        /// Returns the date in local time.
        /// </summary>
        /// <returns>The date in local time.</returns>
        DateTime LocalDate();

        /// <summary>
        /// Returns the date and time in local time.
        /// </summary>
        /// <returns>The date and time in local time.</returns>
        DateTime LocalDateAndTime();

        /// <summary>
        /// Returns a clock that shows the time of the current clock, plus the given offset.
        /// </summary>
        /// <param name="offset">The offset to add to the time of the current clock.</param>
        /// <returns>A clock that shows the time of the current clock, plus the given offset.</returns>
        IClock Add(TimeSpan offset);

        /// <summary>
        /// Returns a clock that shows the time of the current clock, minus the given offset.
        /// </summary>
        /// <param name="offset">The offset to subtract from the time of the current clock.</param>
        /// <returns>A clock that shows the time of the current clock, minus the given offset.</returns>
        IClock Subtract(TimeSpan offset);
    }
}
