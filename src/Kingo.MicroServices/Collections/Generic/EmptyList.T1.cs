﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Collections.Generic
{
    /// <summary>
    /// Represents an empty list.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    public class EmptyList<T> : IReadOnlyList<T>
    {
        /// <inheritdoc />
        public int Count =>
            0;

        /// <inheritdoc />
        public T this[int index] =>
             throw CollectionExtensions.NewIndexOutOfRangeException(index, Count);

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() =>
            Enumerable.Empty<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
