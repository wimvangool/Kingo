using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Represents a page of items of a <see cref="VirtualCollection{T}" />.
    /// </summary>
    /// <typeparam name="T">Type of the items of this collection.</typeparam>
    public class VirtualCollectionPage<T> : ReadOnlyCollection<T>
    {
        private readonly IVirtualCollectionPageLoader<T> _loader;
        private readonly int _pageIndex;
        private readonly List<T> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionPage{T}" /> class.
        /// </summary>
        /// <param name="loader">The loader that loaded this page.</param>
        /// <param name="pageIndex">The index of this page.</param>
        /// <param name="items">The items of this page.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loader"/> or <paramref name="items"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageIndex"/> is less than <c>0</c>.
        /// </exception>
        public VirtualCollectionPage(IVirtualCollectionPageLoader<T> loader, int pageIndex, IEnumerable<T> items)
        {
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }
            if (pageIndex < 0)
            {
                throw NewInvalidPageIndexException(pageIndex);
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            _loader = loader;
            _pageIndex = pageIndex;
            _items = new List<T>(items);
            _items.TrimExcess();
        }

        /// <summary>
        /// The collection this page belongs to.
        /// </summary>
        protected IVirtualCollectionPageLoader<T> Loader
        {
            get { return _loader; }
        }

        /// <summary>
        /// The index of this page.
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
        }

        /// <summary>
        /// Indicates whether or not this page is the first page of the collection.
        /// </summary>
        public bool HasPreviousPage
        {
            get { return PageIndex != 0; }
        }

        /// <summary>
        /// Indicates whether or not this page is the last page of this collection.
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                int count;

                if (Loader.TryGetCount(out count))
                {
                    return (PageIndex + 1) * Loader.PageSize < count;
                }
                return false;
            }
        }

        #region [====== Collection Implementation ======]

        /// <inheritdoc />
        public override int Count
        {
            get { return _items.Count; }
        }

        /// <inheritdoc />
        public override T this[int index]
        {
            get { return this[index, true]; }
        }

        internal T this[int index, bool raiseAccessEvent]
        {
            get
            {
                if (raiseAccessEvent)
                {
                    OnPageItemAccessed(index);
                }
                return _items[index];    
            }
        }

        /// <summary>
        /// Occurs when an item of this page is accessed. This method can be used for prefetching purposes.
        /// </summary>
        /// <param name="index">The index of the item that was accessed.</param>
        protected virtual void OnPageItemAccessed(int index)
        {
            int middleIndex = (_items.Count - 1) / 2;
            if (middleIndex >= index && HasPreviousPage)
            {
                Loader.LoadPage(PageIndex - 1);
            }
            else if (middleIndex < index && HasNextPage)
            {
                Loader.LoadPage(PageIndex + 1);
            }
        }

        /// <inheritdoc />
        protected override int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        /// <inheritdoc />
        protected override bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// <inheritdoc />
        protected override void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        protected override IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewInvalidPageIndexException(int pageIndex)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, pageIndex);
            return new ArgumentOutOfRangeException("pageIndex", message);
        }

        #endregion
    }
}
