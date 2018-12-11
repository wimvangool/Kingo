using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo
{
    /// <summary>
    /// Contains extension methods and helper methods for collection types.
    /// </summary>
    public static class CollectionExtensions
    {
        #region [====== Enumerable ======]

        /// <summary>
        /// Ensures the return-value is always a non-null collection of items. If <paramref name="collection"/>
        /// is <c>null</c>, an empty collection is returned.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection.</typeparam>
        /// <param name="collection">A collection of items.</param>
        /// <returns>A non-null collection.</returns>
        public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T> collection) =>
            collection ?? Enumerable.Empty<T>();

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

        /// <summary>
        /// Attempts to retrieve the first element of the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="collection">The collection to get the element from.</param>
        /// <param name="element">
        /// If this method returns <c>true</c>, this parameter will refer to the first element of the specified <paramref name="collection"/>;
        /// otherwise, it will be set to the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns><c>true</c> if <paramref name="collection"/> contains any elements; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>  
        public static bool TryGetFirstItem<TValue>(this IEnumerable<TValue> collection, out TValue element)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            using (var enumerator = collection.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    element = enumerator.Current;
                    return true;
                }
                element = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Attempts to retrieve the first element of the specified collection that matches the specified <paramref name="predicate"/>. If
        /// <paramref name="predicate"/> is <c>null</c>, the first element of the collection will be returned.
        /// </summary>
        /// <typeparam name="TValue">Type of the element.</typeparam>
        /// <param name="collection">The collection to get the element from.</param>
        /// <param name="predicate">The predicate that will be used to return the correct element.</param>
        /// <param name="element">
        /// If this method returns <c>true</c>, this parameter will refer to the first element of the specified <paramref name="collection"/>;
        /// otherwise, it will be set to the default value of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="collection"/> contains any elements that matches the specified <paramref name="predicate"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>  
        public static bool TryGetFirstItem<TValue>(this IEnumerable<TValue> collection, Func<TValue, bool> predicate, out TValue element)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            using (var enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (Satisfies(enumerator.Current, predicate))
                    {
                        element = enumerator.Current;
                        return true;
                    }                    
                }
                element = default(TValue);
                return false;
            }
        }

        #endregion        

        private static bool Satisfies<TValue>(TValue element, Func<TValue, bool> predicate) =>
            predicate == null || predicate.Invoke(element);

        internal static IndexOutOfRangeException NewIndexOutOfRangeException(int index, int count)
        {
            var messageFormat = ExceptionMessages.ReadOnlyList_IndexOutOfRange;
            var message = string.Format(messageFormat, index, count);
            return new IndexOutOfRangeException(message);
        }
    }
}
