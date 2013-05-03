using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, represents a service that provides the current date and time.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Returns the current date in local time.
        /// </summary>
        /// <returns>The current date in local time.</returns>
        DateTime CurrentDate();

        /// <summary>
        /// Returns the current date and time in local time.
        /// </summary>
        /// <returns>The current date and time in local time.</returns>
        DateTime CurrentDateTime();

        /// <summary>
        /// Returns the current date in universal time.
        /// </summary>
        /// <returns>The current date in universal time.</returns>
        DateTime CurrentDateUtc();

        /// <summary>
        /// Returns the current date and time in universal time.
        /// </summary>
        /// <returns>The current date in universal time.</returns>
        DateTime CurrentDateTimeUtc();
    }
}
