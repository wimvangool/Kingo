using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Represents a list of items that only allows read-operations.
    /// </summary>
    /// <typeparam name="T">Type of the items of this collection.</typeparam>
    public abstract class ReadOnlyCollection<T> : PropertyChangedBase, IList<T>
    {
        #region [====== Count ======]        

        /// <inheritdoc />
        public abstract int Count
        {
            get;
        }

        #endregion

        #region [====== Indexer ======]

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { throw NewNotSupportedException("set_Item"); }
        }

        /// <summary>
        /// Gets the items at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index of the item to retrieve.</param>
        /// <returns>The item at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative or larger than the largest index of this collection.
        /// </exception>
        public abstract T this[int index]
        {
            get;
        }

        #endregion

        #region [====== List - Explicit Read Methods ======]

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        int IList<T>.IndexOf(T item)
        {
            return IndexOf(item);
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, <c>-1.</c></returns>
        protected abstract int IndexOf(T item);

        bool ICollection<T>.Contains(T item)
        {
            return Contains(item);
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        protected abstract bool Contains(T item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from collection.
        /// The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source collection is greater than the available space from
        /// <paramref name="arrayIndex"/> to the end of the destination array.
        /// </exception>
        protected abstract void CopyTo(T[] array, int arrayIndex);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.</returns>
        protected abstract IEnumerator<T> GetEnumerator();               

        #endregion       

        #region [====== List - Explicit Write Methods ======]

        void IList<T>.Insert(int index, T item)
        {
            throw NewNotSupportedException("Index");
        }

        void IList<T>.RemoveAt(int index)
        {
            throw NewNotSupportedException("RemoveAt");
        }

        void ICollection<T>.Add(T item)
        {
            throw NewNotSupportedException("Add");
        }

        void ICollection<T>.Clear()
        {
            throw NewNotSupportedException("Clear");
        }

        bool ICollection<T>.Remove(T item)
        {
            throw NewNotSupportedException("Remove");
        }

        private static Exception NewNotSupportedException(string methodName)
        {
            var messageFormat = ExceptionMessages.ReadOnlyCollection_MethodNotSupported;
            var message = string.Format(messageFormat, methodName);
            return new NotSupportedException(message);
        }

        #endregion
    }
}
