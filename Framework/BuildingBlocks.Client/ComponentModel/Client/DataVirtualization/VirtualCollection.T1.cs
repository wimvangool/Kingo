using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Represents a read-only collection of items that is partially loaded in memory while items are accessed or iterated.
    /// </summary>
    /// <typeparam name="T">Type of the items in this collection.</typeparam>
    public abstract class VirtualCollection<T> : ReadOnlyCollection<VirtualCollectionItem<T>>, INotifyCollectionChanged, IDisposable
    {
        /// <summary>
        /// Defines the default page size.
        /// </summary>
        protected const int DefaultPageSize = 20;

        private readonly VirtualCollectionPageLoader<T> _loader;
        private readonly Lazy<ObjectCache> _pageCache;
        private readonly CollectionChangedEventThrottle _eventThrottle;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollection{T}" /> class.
        /// </summary>  
        protected VirtualCollection()
            : this(DefaultPageSize, CollectionChangedEventThrottle.DefaultRaiseInterval, SynchronizationContext.Current) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollection{T}" /> class.
        /// </summary>               
        /// <param name="collectionChangedInterval">
        /// The sliding window interval that is maintained to raise the <see cref="CollectionChanged" /> event
        /// after a change to the collection occurs to prevent it from raising too frequently.
        /// </param>              
        protected VirtualCollection(TimeSpan collectionChangedInterval)
            : this(DefaultPageSize, collectionChangedInterval, SynchronizationContext.Current) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollection{T}" /> class.
        /// </summary>               
        /// <param name="pageSize">The maximum number of items to retrieve per page.</param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageSize"/> has a negative value.
        /// </exception>
        protected VirtualCollection(int pageSize)
            : this(pageSize, CollectionChangedEventThrottle.DefaultRaiseInterval, SynchronizationContext.Current) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollection{T}" /> class.
        /// </summary>       
        /// <param name="synchronizationContext">The context that is used to raise all events.</param>        
        /// <param name="pageSize">The maximum number of items to retrieve per page.</param>
        /// <param name="collectionChangedInterval">
        /// The sliding window interval that is maintained to raise the <see cref="CollectionChanged" /> event
        /// after a change to the collection occurs to prevent it from raising too frequently.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageSize"/> has a negative value.
        /// </exception>
        protected VirtualCollection(int pageSize, TimeSpan collectionChangedInterval, SynchronizationContext synchronizationContext)
        {            
            _loader = new VirtualCollectionPageLoader<T>(this, synchronizationContext, pageSize);
            _pageCache = new Lazy<ObjectCache>(CreatePageCache);
            _eventThrottle = new CollectionChangedEventThrottle(e => CollectionChanged.Raise(this, e))
            {
                RaiseInterval = collectionChangedInterval
            };
        }
        
        /// <summary>
        /// The maximum number of items that are retrieved per page.
        /// </summary>
        public int PageSize
        {
            get { return _loader.PageSize; }
        }        

        #region [====== Disposable ======]

        /// <summary>
        /// Indicates whether or not this instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing && _pageCache.IsValueCreated)
            {
                var pageCache = _pageCache.Value as IDisposable;
                if (pageCache != null)
                {
                    pageCache.Dispose();
                }
            }
            IsDisposed = true;
        }

        #endregion

        #region [====== List - Count ======]

        /// <summary>
        /// Occurs when the count has been loaded.
        /// </summary>
        public event EventHandler<CountLoadedEventArgs> CountLoaded;

        /// <summary>
        /// Returns the size of this collection.
        /// </summary>
        /// <remarks>
        /// If the size of this collection has not yet been determined, this property will return <c>0</c> and then
        /// trigger an action that will determine the actual size of this collection. 
        /// </remarks>
        public override int Count
        {
            get
            {
                int count;

                if (_loader.TryGetCount(true, out count))
                {
                    return count;
                }
                return 0;
            }              
        }

        /// <summary>
        /// Creates and returns a new <see cref="Task{T}" /> that is loading the size of this collection.
        /// </summary>
        /// <returns>A new <see cref="Task{T}" />.</returns>
        protected internal abstract Task<int> StartLoadCountTask();
        
        /// <summary>
        /// Notifies the <see cref="Count" /> property and the whole collection has changed.
        /// </summary>        
        /// <param name="e">Argument that contains the loaded count value.</param>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        /// <remarks>This method is invoked when the count has been (re)loaded.</remarks>
        protected internal virtual void OnCountLoaded(CountLoadedEventArgs e)
        {
            CountLoaded.Raise(this, e);

            NotifyOfPropertyChange(() => Count);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region [====== List - Indexer ======]

        /// <summary>
        /// Returns the item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>
        /// The loaded item if this item has been loaded, or an error-item if a problem occurred while loading the item.
        /// In any other case, it will return an item that indicates the item is not (yet) loaded.
        /// </returns>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative or larger than largest index of this collection.
        /// </exception>        
        public override VirtualCollectionItem<T> this[int index]
        {
            get { return GetItem(index, true); }      
        }

        /// <summary>
        /// Returns the item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <param name="loadPageIfRequired">
        /// Indicates whether or not the page containing the requested item should be loaded if it was
        /// not already loaded.
        /// </param>               
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative or larger than largest index of this collection.
        /// </exception> 
        protected VirtualCollectionItem<T> GetItem(int index, bool loadPageIfRequired)
        {
            if (index < 0 || index >= Count)
            {
                throw NewIndexOutOfRangeException(index);
            }
            if (_loader.HasFailedToLoadItem(index))
            {
                return new VirtualCollectionItem<T>(index, VirtualCollectionItemStatus.FailedToLoad);
            }
            T item;

            if (_loader.TryGetItem(index, loadPageIfRequired, out item))
            {
                return new VirtualCollectionItem<T>(index, item);
            }
            return new VirtualCollectionItem<T>(index, VirtualCollectionItemStatus.NotLoaded);
        }

        /// <summary>
        /// Notifies a specific page of the collection failed to load (item go from NotLoaded to Error).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the index of the item that failed to load.</param>
        /// <remarks>This method is invoked when an item has failed to load.</remarks>
        protected virtual void HandlePageFailedToLoad(object sender, PageFailedToLoadEventArgs e)
        {            
            OnCollectionChanged();
        }

        /// <summary>
        /// Notifies a specific page of the collection was loaded (items go from NotLoaded to Loaded).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the loaded item.</param>
        /// <remarks>This method is invoked when an item has been loaded.</remarks>
        protected virtual void HandlePageLoaded(object sender, PageLoadedEventArgs<T> e)
        {           
            OnCollectionChanged();
        }       

        #endregion

        #region [====== Read Methods ======]

        /// <inheritdoc />
        protected override int IndexOf(VirtualCollectionItem<T> item)
        {
            int itemIndex = item.Index;
            int count;

            if (0 <= itemIndex && _loader.TryGetCount(false, out count) && itemIndex < count)
            {
                if (item.Equals(GetItem(itemIndex, false)))
                {
                    return itemIndex;
                }
            }
            return -1;
        }

        /// <inheritdoc />
        protected override bool Contains(VirtualCollectionItem<T> item)
        {
            return IndexOf(item) >= 0;
        }

        /// <inheritdoc />
        protected override void CopyTo(VirtualCollectionItem<T>[] array, int arrayIndex)
        {
            var enumerator = GetEnumerator();

            for (int index = arrayIndex; index < array.Length && enumerator.MoveNext(); index++)
            {
                array[index] = enumerator.Current;
            }
        }

        /// <inheritdoc />
        protected override IEnumerator<VirtualCollectionItem<T>> GetEnumerator()
        {
            int count;

            if (_loader.TryGetCount(false, out count))
            {                
                for (int index = 0; index < count; index++)
                {
                    yield return GetItem(index, false);
                }                
            }
        }        

        #endregion

        #region [====== Page Loading ======]

        /// <summary>
        /// Occurs when there was a problem loading a specific page.
        /// </summary>
        public event EventHandler<PageFailedToLoadEventArgs> PageFailedToLoad;

        /// <summary>
        /// Occurs when a page has been loaded.
        /// </summary>
        public event EventHandler<PageLoadedEventArgs<T>> PageLoaded;

        /// <summary>
        /// Raises the <see cref="PageFailedToLoad" /> event.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected internal virtual void OnPageFailedToLoad(PageFailedToLoadEventArgs e)
        {
            PageFailedToLoad.Raise(this, e);

            OnCollectionChanged();
        }

        /// <summary>
        /// Raises the <see cref="PageLoaded" /> event.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected internal virtual void OnPageLoaded(PageLoadedEventArgs<T> e)
        {
            PageLoaded.Raise(this, e);

            OnCollectionChanged();
        }

        /// <summary>
        /// Creates and returns a new <see cref="Task{T}" /> that will load one page of items at the specified <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page to load.</param>
        /// <returns>A new <see cref="Task{T}" />.</returns>
        protected internal abstract Task<IList<T>> StartLoadPageTask(int pageIndex);        

        #endregion

        #region [====== Page Caching ======]

        /// <summary>
        /// Returns the cache that is used to store pages.
        /// </summary>
        protected internal ObjectCache PageCache
        {
            get { return _pageCache.Value; }
        }

        /// <summary>
        /// Creates and returns an <see cref="ObjectCache" /> that will be used to cache loaded pages.
        /// </summary>
        /// <returns>A new <see cref="ObjectCache" /> instance.</returns>
        protected virtual ObjectCache CreatePageCache()
        {
            return new MemoryCache(GetType().Name);
        }        

        /// <summary>
        /// Creates and returns a new <see cref="CacheItemPolicy" /> for the specified <paramref name="pageIndex"/>,
        /// or <c>null</c> if no policy is specified (the default).
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>  
        /// <param name="isErrorPage">
        /// Indicates whether or not the page corresponding to the specified <paramref name="pageIndex"/> failed to load,
        /// in which case the caching policy might differ from that of correctly loaded pages.
        /// </param>      
        /// <returns>A new <see cref="CacheItemPolicy" /> for the specified <paramref name="pageIndex"/>.</returns>        
        protected internal virtual CacheItemPolicy CreatePageCachePolicy(int pageIndex, bool isErrorPage)
        {
            if (isErrorPage)
            {
                // By default, error-pages are kept in memory for only 30 seconds to allow them to be reloaded
                // after some reasonable time.
                return new CacheItemPolicy()
                {
                    AbsoluteExpiration = Clock.Current.UtcDateAndTime().AddSeconds(30)
                };
            }
            return null;
        } 

        #endregion

        #region [====== NotifyCollectionChanged ======]

        /// <summary>
        /// Gets or sets the sliding window interval that is used to prevent the
        /// <see cref="CollectionChanged" />-event from raising too frequently.
        /// </summary>
        public TimeSpan CollectionChangedInterval
        {
            get { return _eventThrottle.RaiseInterval; }
            set { _eventThrottle.RaiseInterval = value; }
        }

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Notifies that one or more items of the collection were replaced.
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
            // NB: We would to use 'Replace' but constructor currently only supports 'Reset'.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _eventThrottle.NotifyCollectionChanged(e);
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        #endregion
    }
}
