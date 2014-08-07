using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Serves as a base-class implementation for implementors of the <see cref="IVirtualCollectionImplementation{T}" /> interface..
    /// </summary>
    /// <typeparam name="T">Type of the items of the collection.</typeparam>
    public abstract class VirtualCollectionImplementation<T> : AsyncObject, IVirtualCollectionImplementation<T>
    {
        private const int _DefaultPageSize = 20;

        private readonly int _pageSize;               
        private readonly IndexSet _pagesThatAreLoading;
        private bool _isLoadingCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionImplementation{T}" /> class.
        /// </summary>
        protected VirtualCollectionImplementation()
            : this(_DefaultPageSize, SynchronizationContext.Current) { }        

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionImplementation{T}" /> class.
        /// </summary>
        /// <param name="pageSize">The size of the chucks of items that are loaded at once.</param>       
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageSize"/> is negative.
        /// </exception>
        protected VirtualCollectionImplementation(int pageSize)
            : this(pageSize, SynchronizationContext.Current) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollectionImplementation{T}" /> class.
        /// </summary>
        /// <param name="pageSize">The size of the chucks of items that are loaded at once.</param>        
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageSize"/> is negative.
        /// </exception>
        protected VirtualCollectionImplementation(int pageSize, SynchronizationContext synchronizationContext)  : base(synchronizationContext)
        {
            if (pageSize < 0)
            {
                throw NewInvalidPageSizeException(pageSize);
            }                       
            _pageSize = pageSize;                      
            _pagesThatAreLoading = new IndexSet(); 
        }

        /// <summary>
        /// Returns the size of the chuncks of items that are loaded at once.
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
        }

        /// <summary>
        /// Returns the cache that is used to store pages.
        /// </summary>
        protected abstract QueryCache PageCache
        {
            get;
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
            if (_isLoadingCount)
            {
                return CountNotYetLoaded(out count);
            }
            _isLoadingCount = true;

            StartLoadCountTask().ContinueWith(task =>
            {
                using (var scope = CreateSynchronizationContextScope())
                {
                    scope.Send(() => _isLoadingCount = false);

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
        public event EventHandler<ItemFailedToLoadEventArgs> ItemFailedToLoad;

        /// <summary>
        /// Raises the <see cref="ItemFailedToLoad" /> event.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnItemFailedToLoad(ItemFailedToLoadEventArgs e)
        {
            ItemFailedToLoad.Raise(this, e);
        }

        /// <inheritdoc />
        public event EventHandler<ItemLoadedEventArgs<T>> ItemLoaded;

        /// <summary>
        /// Raises the <see cref="ItemLoaded" /> event.
        /// </summary>
        /// <param name="e">Argument of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnItemLoaded(ItemLoadedEventArgs<T> e)
        {
            ItemLoaded.Raise(this, e);
        }

        /// <inheritdoc />
        public bool TryGetItem(int index, out T item)
        {
            if (Count.HasValue && (index < 0 || index >= Count.Value))
            {
                throw NewIndexOutOfRangeException(index);
            }
            if (Count.HasValue)
            {
                return TryGetItemThroughPage(index, out item);
            }
            throw NewCountNotLoadedException();
        }

        private bool TryGetItemThroughPage(int index, out T item)
        {
            var pageIndex = index / PageSize;            
            IList<T> page;

            if (TryGetPageFromCache(pageIndex, out page))
            {
                item = page[index % PageSize];
                return true;
            }            
            LoadPage(pageIndex);

            return ItemNotYetLoaded(out item);
        }        

        internal void LoadPage(int pageIndex)
        {
            if (_pagesThatAreLoading.Contains(pageIndex))
            {
                return;
            }
            _pagesThatAreLoading.Add(pageIndex);

            StartLoadPageTask(pageIndex).ContinueWith(task =>
            {
                using (var scope = CreateSynchronizationContextScope())
                {
                    scope.Send(() => _pagesThatAreLoading.Remove(pageIndex));

                    if (task.IsFaulted)
                    {
                        OnPageFailedToLoad(pageIndex);
                    }
                    else
                    {
                        OnPageLoaded(pageIndex, task.Result);
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
        protected abstract Task<VirtualCollectionPage<T>> StartLoadPageTask(int pageIndex);

        private void OnPageFailedToLoad(int pageIndex)
        {
            SynchronizationContextScope.Current.Post(() =>
            {
                var firstIndexOfPage = FirstIndexOf(pageIndex);
                var lastIndexOfPage = LastIndexOf(pageIndex);

                for (int index = firstIndexOfPage; index <= lastIndexOfPage; index++)
                {
                    OnItemFailedToLoad(new ItemFailedToLoadEventArgs(index));
                }
            });            
        }

        private void OnPageLoaded(int pageIndex, VirtualCollectionPage<T> page)
        {
            SynchronizationContextScope.Current.Post(() =>
            {
                AddPageToCache(pageIndex, page);

                var firstIndexOfPage = FirstIndexOf(pageIndex);

                for (int index = 0; index < page.Count; index++)
                {
                    var itemIndex = firstIndexOfPage + index;
                    var item = page[index, false];

                    OnItemLoaded(new ItemLoadedEventArgs<T>(itemIndex, item));
                }
            });
        }

        private int FirstIndexOf(int pageIndex)
        {
            return pageIndex * PageSize;
        }

        private int LastIndexOf(int pageIndex)
        {
            var lastIndexOfCollection = Count.Value - 1;
            var index = (pageIndex + 1) * PageSize - 1;

            return Math.Min(lastIndexOfCollection, index);
        }

        private void AddPageToCache(int pageIndex, IList<T> page)
        {
            PageCache.AddOrUpdate(pageIndex, new QueryCacheValue(page, CreatePageLifetime(pageIndex, page)));
        }

        /// <summary>
        /// Creates and returns a new <see cref="QueryCacheValueLifetime" /> for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="page">The page to cache.</param>
        /// <returns>A new <see cref="QueryCacheValueLifetime" /> for the specified <paramref name="page"/>.</returns>
        protected virtual QueryCacheValueLifetime CreatePageLifetime(int pageIndex, IList<T> page)
        {
            // The first page is kept in memory indefinitely.
            if (pageIndex == 0)
            {
                return null;
            }
            return new SlidingWindowLifetime(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Attempts to retrieve a page of items from cache.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="page">
        /// If the item was found in cache, this parameter will refer to the page after the method has completed.
        /// Will be <c>null</c> otherwise.
        /// </param>
        /// <returns><c>true</c> if the item was found in cache; otherwise <c>false</c>.</returns>
        protected virtual bool TryGetPageFromCache(int pageIndex, out IList<T> page)
        {
            QueryCacheValue cachedPage;

            if (PageCache.TryGetValue(pageIndex, out cachedPage))
            {
                page = cachedPage.Access<IList<T>>();
                return true;
            }
            page = null;
            return false;
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

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        private static Exception NewCountNotLoadedException()
        {
            return new InvalidOperationException(ExceptionMessages.VirtualCollectionImplementation_CountNotLoaded);
        }

        #endregion
    }
}
