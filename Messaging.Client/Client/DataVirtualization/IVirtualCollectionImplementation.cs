using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// When implemented by a class, represents an implementation for the <see cref="VirtualCollection{T}" /> class
    /// that manages loading, caching and cleanup of pages that are (temporarily) kept in memory to mimic a real
    /// collection.
    /// </summary>
    /// <typeparam name="T">Type of the items that are loaded.</typeparam>
    public interface IVirtualCollectionImplementation<T> : IEnumerable<T>
    {
        #region [====== Count ======]

        /// <summary>
        /// Occurs when the count has been loaded.
        /// </summary>
        event EventHandler<CountLoadedEventArgs> CountLoaded;

        /// <summary>
        /// Retrieves the count of an associated <see cref="VirtualCollection{T}" />, or triggers to load it asynchronously.
        /// </summary>
        /// <param name="count">When the count has been loaded before, this parameter will contain the count after the method completes.</param>
        /// <returns><c>true</c> if the count was cached and assigned to <paramref name="count"/>; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If the count has been cached, this method will simply assign the cached value to <paramref name="count"/> and return <c>true</c>.
        /// If not, this method will return <c>false</c> and then trigger an action that loads the count asynchronously. When count has
        /// been loaded, the <see cref="CountLoaded" /> event is raised, such that the collection can update it's <see cref="ICollection{T}.Count">Count</see>
        /// property.
        /// </remarks>
        bool TryGetCount(out int count);

        #endregion

        #region [====== Item Loading ======]

        /// <summary>
        /// Occurs when there was a problem loading a specific item of the collection.
        /// </summary>
        event EventHandler<ItemFailedToLoadEventArgs> ItemFailedToLoad;

        /// <summary>
        /// Occurs when an item has been loaded.
        /// </summary>
        event EventHandler<ItemLoadedEventArgs<T>> ItemLoaded;        

        /// <summary>
        /// Retrieves the item at the specified index of an associated <see cref="VirtualCollection{T}" />, or triggers to load it asynchronously.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <param name="item">When the item has been loaded before, this parameter will contain the item after the method completes.</param>
        /// <returns><c>true</c> if the item was cached and assigned to <paramref name="item"/>; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// The size of the collection has not yet been determined (count has not been loaded).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative or greater than the (current) count of the collection.
        /// </exception>
        /// <remarks>
        /// If the item has been cached, this method will simply assign the cached value to <paramref name="item"/> and return <c>true</c>.
        /// If not, this method will return <c>false</c> and then trigger an action that loads the item asynchronously. When item has
        /// been loaded, the <see cref="ItemLoaded" /> event is raised, such that the collection can update it's collection.        
        /// </remarks>
        bool TryGetItem(int index, out T item);        

        #endregion               
    }
}
