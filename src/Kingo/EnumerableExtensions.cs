using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo
{
    /// <summary>
    /// Contains extension methods for the <see cref="IEnumerable{T}" /> interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns a collection where all <c>null</c> values have been removed.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection.</typeparam>
        /// <param name="collection">A collection of items.</param>
        /// <returns>A subset of the specified <paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> collection)
        {
            return collection.Where(item => !ReferenceEquals(item, null));
        }

        /// <summary>
        /// Attempts to retrieve the <paramref name="element"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="collection">The collection to get the element from.</param>
        /// <param name="index">The index of the element.</param>
        /// <param name="element">
        /// If this method returns <c>true</c>, this parameter will refer to the element at the specified <paramref name="index"/>;
        /// otherwise, it will be set to the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the collection contains an element at the specified <paramref name="index"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>        
        public static bool TryGetItem<TValue>(this IEnumerable<TValue> collection, int index, out TValue element)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (0 <= index)
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    var indexMinusOne = index - 1;
                    var currentIndex = -1;

                    while (currentIndex < indexMinusOne && enumerator.MoveNext())
                    {
                        currentIndex++;
                    }
                    if (enumerator.MoveNext())
                    {
                        element = enumerator.Current;
                        return true;
                    }
                }
            }
            element = default(TValue);
            return false;
        }
    }
}
