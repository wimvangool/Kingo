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
    }
}
