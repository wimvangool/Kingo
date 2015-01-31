using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client.DataVirtualization
{    
    internal sealed class VirtualCollectionPageLoader<T> : AsyncObject
    {        
        private readonly VirtualCollection<T> _collection;
        private readonly int _pageSize;     
        private readonly IndexSet _pagesThatAreLoading;
        private readonly IndexSet _pagesThatFailedToLoad;

        private int? _count;
        private Task<int> _loadCountTask;                
        
        internal VirtualCollectionPageLoader(VirtualCollection<T> collection, SynchronizationContext synchronizationContext, int pageSize)  : base(synchronizationContext)
        {
            if (pageSize < 0)
            {
                throw NewInvalidPageSizeException(pageSize);
            }
            _collection = collection;
            _pageSize = pageSize;                      
            _pagesThatAreLoading = new IndexSet(); 
            _pagesThatFailedToLoad = new IndexSet();
        }
        
        internal int PageSize
        {
            get { return _pageSize; }
        }               

        #region [====== Count ======]        
                
        private void OnCountLoaded(CountLoadedEventArgs e)
        {
            _count = e.Count;            
            _collection.OnCountLoaded(e);
        }        
        
        internal bool TryGetCount(out int count)
        {
            if (_count.HasValue)
            {
                count = _count.Value;
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
        
        private Task<int> StartLoadCountTask()
        {
            return _collection.StartLoadCountTask();
        }

        #endregion

        #region [====== Page Loading ======]

        /// <inheritdoc />
        internal bool TryGetItem(int index, out T item)
        {
            if (_count.HasValue && (index < 0 || index > LargestItemIndex()))
            {
                throw NewInvalidItemIndexException(index);
            }
            if (_count.HasValue)
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
        internal void LoadItem(int index)
        {
            if (index < 0 || index > LargestItemIndex())
            {
                throw NewInvalidItemIndexException(index);
            }
            LoadPage(CalculatePageIndex(index));
        }

        /// <inheritdoc />
        internal void LoadPage(int pageIndex)
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
        
        private Task<IList<T>> StartLoadPageTask(int pageIndex)
        {
            return _collection.StartLoadPageTask(pageIndex);
        }
        
        private VirtualCollectionPage<T> CreatePage(int pageIndex, IEnumerable<T> items)
        {
            return new VirtualCollectionPage<T>(this, pageIndex, items);
        }

        private void OnPageFailedToLoad(int pageIndex)
        {
            _pagesThatFailedToLoad.Add(pageIndex);

            SynchronizationContextScope.Current.Post(() => OnPageFailedToLoad(new PageFailedToLoadEventArgs(pageIndex)));            
        }        
        
        private void OnPageFailedToLoad(PageFailedToLoadEventArgs e)
        {
            _collection.OnPageFailedToLoad(e);
        }

        private void OnPageLoaded(int pageIndex, VirtualCollectionPage<T> page)
        {
            AddPageToCache(pageIndex, page);

            _pagesThatAreLoading.Remove(pageIndex);

            SynchronizationContextScope.Current.Post(() => OnPageLoaded(new PageLoadedEventArgs<T>(page)));
        }       
        
        private void OnPageLoaded(PageLoadedEventArgs<T> e)
        {
            _collection.OnPageLoaded(e);
        }

        #endregion

        #region [====== Page Caching ======]        
        
        private ObjectCache PageCache
        {
            get { return _collection.PageCache; }
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
        
        private string CacheItemKeyOf(int pageIndex)
        {
            return _collection.CacheItemKeyOf(pageIndex);
        }
        
        private CacheItemPolicy CreatePageCachePolicy(int pageIndex)
        {
            // The first page is always kept in memory indefinitely.
            return pageIndex == 0 ? null : _collection.CreatePageCachePolicy(pageIndex);            
        }        

        #endregion

        #region [====== Probe Methods ======]
        
        internal bool HasLoadedItem(int index)
        {
            if (index < 0 || index > LargestItemIndex())
            {
                return false;
            }
            return HasLoadedPage(CalculatePageIndex(index));
        }
       
        internal bool HasFailedToLoadItem(int index)
        {
            if (index < 0 || index > LargestItemIndex())
            {
                return false;
            }
            return HasFailedToLoadPage(CalculatePageIndex(index));
        }
        
        internal bool HasLoadedPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > LargestPageIndex())
            {
                return false;
            }
            return HasCachedPage(pageIndex);
        }
        
        internal bool HasFailedToLoadPage(int pageIndex)
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
            if (_count.HasValue)
            {
                return _count.Value - 1;
            }
            return -1;
        }

        private int LargestPageIndex()
        {
            if (_count.HasValue)
            {
                return (int) Math.Ceiling((double) _count.Value / PageSize);
            }
            return -1;
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
