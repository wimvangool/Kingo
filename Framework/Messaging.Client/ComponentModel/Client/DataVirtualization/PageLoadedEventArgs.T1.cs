using System;
using System.Collections.Generic;

namespace Syztem.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="VirtualCollection{T}.PageLoaded" /> event.
    /// </summary>
    /// <typeparam name="T">Type of the item that was loaded.</typeparam>
    public class PageLoadedEventArgs<T> : EventArgs
    {        
        /// <summary>
        /// The page that was loaded.
        /// </summary>
        private readonly VirtualCollectionPage<T> _page;
        
        internal PageLoadedEventArgs(VirtualCollectionPage<T> page)
        {                       
            _page = page;
        } 
       
        /// <summary>
        /// The index of the page that was loaded.
        /// </summary>
        public int PageIndex
        {
            get { return _page.PageIndex; }
        }

        /// <summary>
        /// The amount of items in the page.
        /// </summary>
        public int PageSize
        {
            get { return _page.Count; }
        }

        /// <summary>
        /// Indicates whether or not this page is the first page of the collection.
        /// </summary>
        public bool HasPreviousPage
        {
            get { return _page.HasPreviousPage; }
        }

        /// <summary>
        /// Indicates whether or not this page is the last page of the collection.
        /// </summary>
        public bool HasNextPage
        {
            get { return _page.HasNextPage; }
        }

        /// <summary>
        /// The items of the page that was loaded.
        /// </summary>
        public IEnumerable<T> Items
        {
            get { return _page; }
        }
    }
}
