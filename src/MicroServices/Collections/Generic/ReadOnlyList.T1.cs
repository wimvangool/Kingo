using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Collections.Generic
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
        public virtual T this[int index]
        {
            get
            {
                if (0 <= index && index < Count)
                {
                    return this.ElementAt(index);
                }
                throw CollectionExtensions.NewIndexOutOfRangeException(index, Count);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public abstract IEnumerator<T> GetEnumerator();        
    }
}
