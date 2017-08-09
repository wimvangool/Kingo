using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Resources;

namespace Kingo
{
    /// <summary>
    /// Contains extension methods and helper methods for collection types.
    /// </summary>
    public static class CollectionExtensions
    {
        #region [====== Enumerable ======]

        /// <summary>
        /// Returns a collection where all <c>null</c> values have been removed.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection.</typeparam>
        /// <param name="collection">A collection of items.</param>
        /// <returns>A subset of the specified <paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> collection) =>
            collection.Where(item => !ReferenceEquals(item, null));

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

        #endregion

        #region [====== Array ======]

        /// <summary>
        /// Creates and returns a new array containing all the elements of the source plus the element to add at the last index.
        /// </summary>
        /// <typeparam name="TElement">Type of the elements in the array.</typeparam>
        /// <param name="elements">An array of elements.</param>
        /// <param name="element">The element to add at the end of the new array.</param>
        /// <returns>A new array with the element at the last index.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="elements"/> is <c>null</c>.
        /// </exception>
        public static TElement[] Add<TElement>(this TElement[] elements, TElement element)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }
            var elementsPlusOne = new TElement[elements.Length + 1];

            Array.Copy(elements, 0, elementsPlusOne, 0, elements.Length);

            elementsPlusOne[elements.Length] = element;

            return elementsPlusOne;
        }

        #endregion

        internal static IndexOutOfRangeException NewIndexOutOfRangeException(int index, int count)
        {
            var messageFormat = ExceptionMessages.ReadOnlyList_IndexOutOfRange;
            var message = string.Format(messageFormat, index, count);
            return new IndexOutOfRangeException(message);
        }
    }
}
