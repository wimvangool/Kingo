using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client.DataVirtualization
{
    internal sealed class VirtualCollectionPageLoaderSpy : VirtualCollectionPageLoader<int>, IDisposable
    {
        #region [====== PageLifetime ======]

        private sealed class CachedPagePolicy : CacheItemPolicy
        {
            private readonly ManualResetEventSlim _removedFromCacheLock;

            public CachedPagePolicy(TimeSpan timeout)
            {
                _removedFromCacheLock = new ManualResetEventSlim();

                AbsoluteExpiration = new DateTimeOffset(Clock.Current.LocalDateAndTime().Add(timeout));
                RemovedCallback = NotifyRemovedFromCache;                
            }            

            private void NotifyRemovedFromCache(CacheEntryRemovedArguments arguments)
            {                
                _removedFromCacheLock.Set();
            }

            public bool WaitUntilRemovedFromCache(TimeSpan timeout)
            {
                if (_removedFromCacheLock.Wait(timeout))
                {
                    _removedFromCacheLock.Dispose();
                    return true;
                }
                return false;
            }
        }

        #endregion

        private readonly List<int> _items;
        private readonly MemoryCache _pageCache;
        private readonly Dictionary<int, CachedPagePolicy> _cachedPagePolicies;
        private readonly AutoResetEvent _pageLoadWaitEvent;
        private bool _isDisposed;

        public VirtualCollectionPageLoaderSpy(IEnumerable<int> items, int pageSize) : base(Guid.NewGuid(), pageSize)
        {
            _items = new List<int>(items);
            _pageCache = new MemoryCache("VirtualCollectionCache");
            _cachedPagePolicies = new Dictionary<int, CachedPagePolicy>();
            _pageLoadWaitEvent = new AutoResetEvent(false);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _pageCache.Dispose();
            _pageLoadWaitEvent.Dispose();
            _isDisposed = true;
        }

        protected override ObjectCache PageCache
        {
            get { return _pageCache; }
        }

        public bool FailNextPageLoad
        {
            get;
            set;
        }

        public bool WaitForPageLoadSignal
        {
            get;
            set;
        }

        public void SignalPageLoadToContinue()
        {
            _pageLoadWaitEvent.Set();
        }

        public bool UseInfiniteCacheLifetime
        {
            get;
            set;
        }

        public int LoadCountInvocations
        {
            get;
            private set;
        }       

        public int LoadPageInvocations
        {
            get;
            private set;
        }

        protected override Task<int> StartLoadCountTask()
        {
            LoadCountInvocations++;
            
            return CreateCompletedTask(_items.Count);
        }

        public bool WaitUntilRemovedFromCache(int index, TimeSpan timeout)
        {
            int pageIndex = index / PageSize;
            CachedPagePolicy lifetime;

            if (_cachedPagePolicies.TryGetValue(pageIndex, out lifetime))
            {
                return lifetime.WaitUntilRemovedFromCache(timeout);
            }
            return false;
        }        

        protected override CacheItemPolicy CreatePageCachePolicy(int pageIndex)
        {
            if (UseInfiniteCacheLifetime)
            {
                return null;
            }
            var policy = new CachedPagePolicy(TimeSpan.FromMilliseconds(100));

            _cachedPagePolicies[pageIndex] = policy;

            return policy;
        }        

        protected override Task<IList<int>> StartLoadPageTask(int pageIndex)
        {
            LoadPageInvocations++;

            if (FailNextPageLoad)
            {
                return CreateFailedTask();
            }           
            if (WaitForPageLoadSignal)
            {
                return Task<IList<int>>.Factory.StartNew(() =>
                {
                    if (_pageLoadWaitEvent.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        return LoadPageCore(pageIndex);
                    }
                    return Fail();
                });
            }
            return CreateCompletedTask(LoadPageCore(pageIndex));
        }

        private IList<int> LoadPageCore(int pageIndex)
        {
            var page = new List<int>(PageSize);
            var firstIndex = pageIndex * PageSize;
            var lastIndex = Math.Min((pageIndex + 1) * PageSize - 1, _items.Count - 1);

            for (int index = firstIndex; index <= lastIndex; index++)
            {
                page.Add(_items[index]);
            }
            return new VirtualCollectionPage<int>(this, pageIndex, page);
        }

        private static Task<TResult> CreateCompletedTask<TResult>(TResult result)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
        }

        private static Task<IList<int>> CreateFailedTask()
        {
            using (var handle = new ManualResetEventSlim(false, 1000))
            {
                var task = Task<IList<int>>.Factory.StartNew(Fail);

                task.ContinueWith(t => handle.Set(), TaskContinuationOptions.ExecuteSynchronously);

                handle.Wait();

                return task;
            }
        }

        private static IList<int> Fail()
        {
            throw new Exception("Failed to load page.");
        }
    }
}
