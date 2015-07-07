using System;
using Syztem.Resources;

namespace Syztem.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="VirtualCollection{T}.CountLoaded" /> event.
    /// </summary>
    public class CountLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// The count that has been loaded.
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountLoadedEventArgs" /> class.
        /// </summary>
        /// <param name="count">The count that has been loaded.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> is negative.
        /// </exception>
        public CountLoadedEventArgs(int count)
        {
            if (count < 0)
            {
                throw NewInvalidCountException(count);
            }
            Count = count;
        }

        private static Exception NewInvalidCountException(int count)
        {
            var messageFormat = ExceptionMessages.CountLoadedEventArgs_InvalidCount;
            var message = string.Format(messageFormat, count);
            return new ArgumentOutOfRangeException("count", message);
        }
    }
}
