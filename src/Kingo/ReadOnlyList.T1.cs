using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Kingo.CollectionExtensions;

namespace Kingo
{
    /// <summary>
    /// Provides a base class implementation for all classes that implement the <see cref="IReadOnlyList{T}" /> interface.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    public abstract class ReadOnlyList<T> : IReadOnlyList<T>
    {
        /// <inheritdoc />
        public abstract int Count
        {
            get;
        }

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                if (0 <= index && index < Count)
                {
                    return GetItem(index);
                }
                throw NewIndexOutOfRangeException(index, Count);
            }
        }

        /// <summary>
        /// Returns the item at the specified <paramref name="index"/>, where <paramref name="index"/> has been range-checked.
        /// By default, this method simply iterates the list, but implementing classes can optimize this method by overriding it.
        /// </summary>
        /// <param name="index">Index of the item to return.</param>
        /// <returns>The item at the specified <paramref name="index"/>.</returns>
        protected virtual T GetItem(int index)
        {
            return this.ElementAt(index);
        }

        /// <inheritdoc />
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        
    }
}
