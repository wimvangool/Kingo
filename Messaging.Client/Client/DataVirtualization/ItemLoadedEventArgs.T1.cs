using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="IVirtualCollectionImplementation{T}.ItemLoaded" /> event.
    /// </summary>
    /// <typeparam name="T">Type of the item that was loaded.</typeparam>
    public class ItemLoadedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The index of the loaded item.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The item that was loaded.
        /// </summary>
        public readonly T Item;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLoadedEventArgs{T}" /> class.
        /// </summary>
        /// <param name="index">The index of the loaded item.</param>
        /// <param name="item">The item that was loaded.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public ItemLoadedEventArgs(int index, T item)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            Index = index;
            Item = item;
        }

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }
    }
}
