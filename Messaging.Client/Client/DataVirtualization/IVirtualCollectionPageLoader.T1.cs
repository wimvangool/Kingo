using System.Collections.Generic;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// When implemented by a class, represents an implementation for the <see cref="VirtualCollection{T}" /> class
    /// that manages loading, caching and cleanup of pages that are (temporarily) kept in memory to mimic a real
    /// collection.
    /// </summary>
    /// <typeparam name="T">Type of the items that are loaded.</typeparam>
    public interface IVirtualCollectionPageLoader<T> : IEnumerable<T>
    {
        /// <summary>
        /// Returns the size of the chunks of items that are loaded at once.
        /// </summary>
        int PageSize
        {
            get;
        }

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
        /// Occurs when there was a problem loading a specific page.
        /// </summary>
        event EventHandler<PageFailedToLoadEventArgs> PageFailedToLoad;

        /// <summary>
        /// Occurs when a page has been loaded.
        /// </summary>
        event EventHandler<PageLoadedEventArgs<T>> PageLoaded;        

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
        /// If the item has been loaded, this method will simply assign the cached value to <paramref name="item"/> and return <c>true</c>.
        /// If not, this method will return <c>false</c> and then trigger an action that loads the item asynchronously. When item has
        /// been loaded, the <see cref="PageLoaded" /> event is raised, such that the collection can update it's collection.        
        /// </remarks>
        bool TryGetItem(int index, out T item);

        /// <summary>
        /// Forces the page the specified item belongs to to be loaded, if it hasn't already been loaded.
        /// </summary>
        /// <param name="index">The index of the item to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0 or larger than the largest index of this collection.
        /// </exception>
        void LoadItem(int index);

        /// <summary>
        /// Forces the page with index <paramref name="pageIndex"/> to be loaded, if it hasn't already been loaded.
        /// </summary>
        /// <param name="pageIndex">The index of the page to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageIndex"/> is less than 0 or larger than the largest page-index of this collection.
        /// </exception>
        void LoadPage(int pageIndex);        

        #endregion               

        #region [====== Probe Methods ======]

        /// <summary>
        /// Determines whether the item at the specified <paramref name="index"/> has been loaded.
        /// </summary>
        /// <param name="index">The index of the item (relative to the whole collection).</param>
        /// <returns><c>true</c> if the page to which the specified item belongs to was loaded; otherwise <c>false</c>.</returns>        
        bool HasLoadedItem(int index);

        /// <summary>
        /// Determines whether the item at the specified <paramref name="index"/> has failed to load.
        /// </summary>
        /// <param name="index">The index of the item (relative to the whole collection).</param>
        /// <returns><c>true</c> if the page to which the specified item belongs to has failed to load; otherwise <c>false</c>.</returns>  
        bool HasFailedToLoadItem(int index);

        /// <summary>
        /// Determines whether the item at the specified <paramref name="pageIndex"/> has been loaded.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <returns><c>true</c> if the page was loaded; otherwise <c>false</c>.</returns> 
        bool HasLoadedPage(int pageIndex);

        /// <summary>
        /// Determines whether the item at the specified <paramref name="pageIndex"/> has failed to load.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <returns><c>true</c> if the page has failed to load; otherwise <c>false</c>.</returns> 
        bool HasFailedToLoadPage(int pageIndex);

        #endregion
    }
}
