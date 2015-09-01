using System;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// When implemented by a class, represents a service that provides the current date and time.
    /// </summary>
    public interface IClock
    {
        #region [====== Local Time ======]

        /// <summary>
        /// Returns the time of the day in local time.
        /// </summary>
        /// <returns>The time of the day in local time.</returns>
        TimeSpan LocalTime();

        /// <summary>
        /// Returns the date in local time.
        /// </summary>
        /// <returns>The date in local time.</returns>
        DateTimeOffset LocalDate();

        /// <summary>
        /// Returns the date and time in local time.
        /// </summary>
        /// <returns>The date and time in local time.</returns>
        DateTimeOffset LocalDateAndTime();

        #endregion

        #region [====== UTC Time ======]

        /// <summary>
        /// Returns the time of the day in UTC-time.
        /// </summary>
        /// <returns>Time of the day in UTC-time.</returns>
        TimeSpan UtcTime();

        /// <summary>
        /// Returns the date in UTC-time.
        /// </summary>
        /// <returns>Date in UTC-time.</returns>
        DateTimeOffset UtcDate();

        /// <summary>
        /// Returns the date and time in UTC-time.
        /// </summary>
        /// <returns>Date and time in UTC-time.</returns>
        DateTimeOffset UtcDateAndTime();

        #endregion

        /// <summary>
        /// Returns a clock that shows the time of the current clock, plus the given offset.
        /// </summary>
        /// <param name="offset">The offset to add to the time of the current clock.</param>
        /// <returns>A clock that shows the time of the current clock, plus the given offset.</returns>
        IClock Add(TimeSpan offset);        
    }
}
