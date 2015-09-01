using System.Collections.Generic;

namespace ServiceComponents.ComponentModel.Client.DataVirtualization
{    
    internal sealed class VirtualCollectionPage<T> : ReadOnlyCollection<T>
    {
        private readonly VirtualCollectionPageLoader<T> _loader;
        private readonly int _pageIndex;
        private readonly List<T> _items;
        
        internal VirtualCollectionPage(VirtualCollectionPageLoader<T> loader, int pageIndex, IEnumerable<T> items)
        {            
            _loader = loader;
            _pageIndex = pageIndex;
            _items = new List<T>(items);
            _items.TrimExcess();
        }        
       
        internal int PageIndex
        {
            get { return _pageIndex; }
        }
        
        internal bool HasPreviousPage
        {
            get { return PageIndex != 0; }
        }
        
        internal bool HasNextPage
        {
            get
            {
                int count;

                if (_loader.TryGetCount(false, out count))
                {
                    return (PageIndex + 1) * _loader.PageSize < count;
                }
                return false;
            }
        }

        #region [====== Collection Implementation ======]
        
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
        
        private void OnPageItemAccessed(int index)
        {
            int middleIndex = (_items.Count - 1) / 2;
            if (middleIndex >= index && HasPreviousPage)
            {
                _loader.LoadPage(PageIndex - 1);
            }
            else if (middleIndex < index && HasNextPage)
            {
                _loader.LoadPage(PageIndex + 1);
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
    }
}
