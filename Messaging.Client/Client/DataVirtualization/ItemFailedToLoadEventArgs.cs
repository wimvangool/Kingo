using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="IVirtualCollectionImplementation{T}.ItemFailedToLoad" /> event.
    /// </summary>
    public class ItemFailedToLoadEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the item that failed to load.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFailedToLoadEventArgs" /> class.
        /// </summary>
        /// <param name="index">The index of the item that failed to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public ItemFailedToLoadEventArgs(int index)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            Index = index;
        }

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }
    }
}
