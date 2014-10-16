using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Serves as a base-class implementation for implementors of the <see cref="IVirtualCollectionPageLoader{T}" /> interface..
    /// </summary>
    /// <typeparam name="T">Type of the items of the collection.</typeparam>
    /// <remarks>
    /// <para>
    /// The <see cref="VirtualCollectionPageLoader{T}" /> class uses an <see cref="ObjectCache" /> to store
    /// and retrieve it loaded pages. Derived classes are responsible for providing the exact <see cref="ObjectCache" />
    /// implementation by overriding the <see cref="VirtualCollectionPageLoader{T}.PageCache" /> property.    
    /// </para>
    /// <para>
    /// Since an <see cref="ObjectCache"/> implements <see cref="IDisposable" />, the cache must be disposed after use.
    /// However, it is recommended not implement <see cref="IDisposable" /> of the derived class, but to let the client
    /// of the derived implementation provide the cache to the implementation and let it remain the responsibility of
    /// the client to dispose it when necessary. This allows a single cache to be used by multiple components of the
    /// application, and also let pages be cached even after an implementation-instance is no longer used, e.g., when
    /// a screen displaying the items is closed. Then, when the screen is opened again, a new
    /// <see cref="VirtualCollectionPageLoader{T}" /> instance can reuse the cached page of a previous instance,
    /// improving overal performance. For this to work, instances that want to share cached pages need to use
    /// the same <see cref="VirtualCollectionPageLoader{T}.CacheRegionId" />, such that the keys generated for
    /// specific pages are identical.
    /// </para>
    /// </remarks>
    public abstract class VirtualCollectionPageLoader<T> : AsyncObject, IVirtualCollectionPageLoader<T>
    {
        private const int _DefaultPageSize = 20;

        private readonly Guid _cacheRegionId;
        private readonly int _pageSize;               
        private readonly IndexSet _pagesThatAreLoading;
        private readonly IndexSet _pagesThatFailedToLoad;
        private Task<int> _loadCountTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionPageLoader{T}" /> class.
        /// </summary>
        /// <param name="cacheRegionId">
        /// A unique identifier that is used to create the keys to store and retrieve pages from the cache.
        /// </param>
        protected VirtualCollectionPageLoader(Guid cacheRegionId)
            : this(cacheRegionId, _DefaultPageSize, SynchronizationContext.Current) { }        

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionPageLoader{T}" /> class.
        /// </summary>
        /// <param name="cacheRegionId">
        /// A unique identifier that is used to create the keys to store and retrieve pages from the cache.
        /// </param>
        /// <param name="pageSize">The size of the chucks of items that are loaded at once.</param>       
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageSize"/> is negative.
        /// </exception>
        protected VirtualCollectionPageLoader(Guid cacheRegionId, int pageSize)
            : this(cacheRegionId, pageSize, SynchronizationContext.Current) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionPageLoader{T}" /> class.
        /// </summary>
        /// <param name="cacheRegionId">
        /// A unique identifier that is used to create the keys to store and retrieve pages from the cache.
        /// </param>
        /// <param name="pageSize">The size of the chucks of items that are loaded at once.</param>        
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageSize"/> is negative.
        /// </exception>
        protected VirtualCollectionPageLoader(Guid cacheRegionId, int pageSize, SynchronizationContext synchronizationContext)  : base(synchronizationContext)
        {
            if (pageSize < 0)
            {
                throw NewInvalidPageSizeException(pageSize);
            }
            _cacheRegionId = cacheRegionId;      
            _pageSize = pageSize;                      
            _pagesThatAreLoading = new IndexSet(); 
            _pagesThatFailedToLoad = new IndexSet();
        }

        /// <inheritdoc />
        public int PageSize
        {
            get { return _pageSize; }
        }               

        #region [====== Count ======]

        /// <inheritdoc />
        public event EventHandler<CountLoadedEventArgs> CountLoaded;

        /// <summary>
        /// Raises the <see cref="CountLoaded" /> event.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnCountLoaded(CountLoadedEventArgs e)
        {
            Count = e.Count;
            CountLoaded.Raise(this, e);
        }

        /// <summary>
        /// If loaded, returns the size of the collection; returns <c>null</c> otherwise.
        /// </summary>
        protected int? Count
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public bool TryGetCount(out int count)
        {
            if (Count.HasValue)
            {
                count = Count.Value;
                return true;
            }
            if (_loadCountTask != null && !_loadCountTask.IsCompleted)
            {
                return CountNotYetLoaded(out count);
            }            
            _loadCountTask = StartLoadCountTask();
            _loadCountTask.ContinueWith(task =>
            {
                using (var scope = CreateSynchronizationContextScope())
                {                    
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        return;
                    }
                    scope.Post(() => OnCountLoaded(new CountLoadedEventArgs(task.Result)));
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return CountNotYetLoaded(out count);
        }

        private static bool CountNotYetLoaded(out int count)
        {
            count = 0;
            return false;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Task{T}" /> that is loading the size of this collection.
        /// </summary>
        /// <returns>A new <see cref="Task{T}" />.</returns>
        protected abstract Task<int> StartLoadCountTask();

        #endregion

        #region [====== Page Loading ======]

        /// <inheritdoc />
        public bool TryGetItem(int index, out T item)
        {
            if (Count.HasValue && (index < 0 || index > LargestItemIndex()))
            {
                throw NewInvalidItemIndexException(index);
            }
            if (Count.HasValue)
            {
                return TryGetItemThroughPage(index, out item);
            }
            throw NewCountNotLoadedException();
        }

        private bool TryGetItemThroughPage(int index, out T item)
        {
            var pageIndex = CalculatePageIndex(index);            
            IList<T> page;

            if (TryGetPageFromCache(pageIndex, out page))
            {
                item = page[index % PageSize];
                return true;
            }            
            LoadPage(pageIndex, false);

            return ItemNotYetLoaded(out item);
        }

        /// <inheritdoc />
        public void LoadItem(int index)
        {
            if (index < 0 || index > LargestItemIndex())
            {
                throw NewInvalidItemIndexException(index);
            }
            LoadPage(CalculatePageIndex(index));
        }

        /// <inheritdoc />
        public void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > LargestPageIndex())
            {
                throw NewInvalidPageIndexException(pageIndex);
            }
            LoadPage(pageIndex, true);
        }

        private void LoadPage(int pageIndex, bool consultCache)
        {
            if ((consultCache && HasCachedPage(pageIndex)) || _pagesThatAreLoading.Contains(pageIndex) || _pagesThatFailedToLoad.Contains(pageIndex))
            {
                return;
            }
            _pagesThatAreLoading.Add(pageIndex);

            StartLoadPageTask(pageIndex).ContinueWith(task =>
            {
                using (CreateSynchronizationContextScope())
                {                    
                    // NB: We intentionally ignore the IsCanceled-status here,
                    // since if anyone cancelled loading this page, it's as if
                    // nothing ever happened.
                    if (task.IsFaulted)
                    {
                        OnPageFailedToLoad(pageIndex);
                    }
                    else
                    {
                        OnPageLoaded(pageIndex, CreatePage(pageIndex, task.Result));
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously); 
        }
        
        private static bool ItemNotYetLoaded(out T item)
        {
            item = default(T);
            return false;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Task{T}" /> that will load one page of items at the specified <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page to load.</param>
        /// <returns>A new <see cref="Task{T}" />.</returns>
        protected abstract Task<IList<T>> StartLoadPageTask(int pageIndex);

        /// <summary>
        /// Creates and returns a new <see cref="VirtualCollectionPage{T}" /> based on the specified
        /// <paramref name="pageIndex"/> and <paramref name="items"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="items">The items of the page.</param>
        /// <returns>A new <see cref="VirtualCollectionPage{T}" />.</returns>
        protected virtual VirtualCollectionPage<T> CreatePage(int pageIndex, IList<T> items)
        {
            return new VirtualCollectionPage<T>(this, pageIndex, items);
        }

        private void OnPageFailedToLoad(int pageIndex)
        {
            _pagesThatFailedToLoad.Add(pageIndex);

            SynchronizationContextScope.Current.Post(() => OnPageFailedToLoad(new PageFailedToLoadEventArgs(pageIndex)));            
        }

        /// <inheritdoc />
        public event EventHandler<PageFailedToLoadEventArgs> PageFailedToLoad;

        /// <summary>
        /// Raises the <see cref="PageFailedToLoad" /> event.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnPageFailedToLoad(PageFailedToLoadEventArgs e)
        {
            PageFailedToLoad.Raise(this, e);
        }

        private void OnPageLoaded(int pageIndex, VirtualCollectionPage<T> page)
        {
            AddPageToCache(pageIndex, page);

            _pagesThatAreLoading.Remove(pageIndex);

            SynchronizationContextScope.Current.Post(() => OnPageLoaded(new PageLoadedEventArgs<T>(page)));
        }

        /// <inheritdoc />
        public event EventHandler<PageLoadedEventArgs<T>> PageLoaded;

        /// <summary>
        /// Raises the <see cref="PageLoaded" /> event.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnPageLoaded(PageLoadedEventArgs<T> e)
        {
            PageLoaded.Raise(this, e);
        }

        #endregion

        #region [====== Page Caching ======]

        /// <summary>
        /// Returns the identifier that is used by this instance to generate keys for the cached pages.
        /// </summary>
        protected Guid CacheRegionId
        {
            get { return _cacheRegionId; }
        }

        /// <summary>
        /// Returns the cache that is used to store pages.
        /// </summary>
        protected abstract ObjectCache PageCache
        {
            get;
        }

        private bool TryGetPageFromCache(int pageIndex, out IList<T> page)
        {
            return PageCache.TryGetValue(CacheItemKeyOf(pageIndex), out page);
        }

        private bool HasCachedPage(int pageIndex)
        {
            return PageCache.Contains(CacheItemKeyOf(pageIndex));
        }

        private void AddPageToCache(int pageIndex, IList<T> page)
        {
            if (PageCache.Add(CacheItemKeyOf(pageIndex), page, CreatePageCachePolicy(pageIndex)))
            {
                return;
            }
            throw NewPageAlreadyCachedException(pageIndex);
        }

        /// <summary>
        /// Returns the key that is used to store or retrieve a specific page in or from cache.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns>The key that is used to store or retrieve a specific page in or from cache.</returns>
        private string CacheItemKeyOf(int pageIndex)
        {
            return string.Format("{0}.{1}", CacheRegionId, pageIndex);
        }

        /// <summary>
        /// Creates and returns a new <see cref="CacheItemPolicy" /> for the specified <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>        
        /// <returns>A new <see cref="CacheItemPolicy" /> for the specified <paramref name="pageIndex"/>.</returns>
        /// <remarks>
        /// The default implementation keeps the page with <paramref name="pageIndex"/> 0 in cache indefinitely.
        /// For all other pages, a sliding window is used of 30 seconds.
        /// </remarks>
        protected virtual CacheItemPolicy CreatePageCachePolicy(int pageIndex)
        {
            // The first page is kept in memory indefinitely.
            if (pageIndex == 0)
            {
                return new CacheItemPolicy();
            }
            return new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };
        }        

        #endregion

        #region [====== Probe Methods ======]

        /// <inheritdoc />       
        public bool HasLoadedItem(int index)
        {
            if (index < 0 || index > LargestItemIndex())
            {
                return false;
            }
            return HasLoadedPage(CalculatePageIndex(index));
        }

        /// <inheritdoc />
        public bool HasFailedToLoadItem(int index)
        {
            if (index < 0 || index > LargestItemIndex())
            {
                return false;
            }
            return HasFailedToLoadPage(CalculatePageIndex(index));
        }

        /// <inheritdoc />
        public bool HasLoadedPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > LargestPageIndex())
            {
                return false;
            }
            return HasCachedPage(pageIndex);
        }

        /// <inheritdoc />
        public bool HasFailedToLoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > LargestPageIndex())
            {
                return false;
            }
            return _pagesThatFailedToLoad.Contains(pageIndex);
        }

        private int CalculatePageIndex(int index)
        {
            return index / PageSize;
        }

        private int LargestItemIndex()
        {
            if (Count.HasValue)
            {
                return Count.Value - 1;
            }
            return -1;
        }

        private int LargestPageIndex()
        {
            if (Count.HasValue)
            {
                return (int) Math.Ceiling((double) Count.Value / PageSize);
            }
            return -1;
        }

        #endregion

        #region [====== Enumeration ======]

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewInvalidPageSizeException(int pageSize)
        {
            var messageFormat = ExceptionMessages.VirtualCollectionImplementation_InvalidPageSize;
            var message = string.Format(messageFormat, pageSize);
            return new ArgumentOutOfRangeException("pageSize", message);
        }

        private static Exception NewInvalidItemIndexException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }        

        private static Exception NewCountNotLoadedException()
        {
            return new InvalidOperationException(ExceptionMessages.VirtualCollectionImplementation_CountNotLoaded);
        }

        private static Exception NewInvalidPageIndexException(int pageIndex)
        {
            var messageFormat = ExceptionMessages.VirtualCollectionImplementation_InvalidPageIndex;
            var message = string.Format(messageFormat, pageIndex);
            return new ArgumentOutOfRangeException("pageIndex", message);
        }

        private static Exception NewPageAlreadyCachedException(int pageIndex)
        {
            var messageFormat = ExceptionMessages.VirtualCollectionImplementation_PageAlreadyInCache;
            var message = string.Format(messageFormat, pageIndex);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
