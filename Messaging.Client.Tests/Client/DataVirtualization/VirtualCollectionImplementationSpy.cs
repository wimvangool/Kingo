using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    internal sealed class VirtualCollectionImplementationSpy : VirtualCollectionImplementation<int>
    {
        #region [====== PageLifetime ======]

        private sealed class PageLifetime : SlidingWindowLifetime
        {
            private readonly ManualResetEventSlim _removedFromCacheLock;

            public PageLifetime(TimeSpan timeout) : base(timeout)
            {
                _removedFromCacheLock = new ManualResetEventSlim();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _removedFromCacheLock.Dispose();
                }
                base.Dispose(disposing);
            }

            public void NotifyRemovedFromCache()
            {
                _removedFromCacheLock.Set();
            }

            public bool WaitUntilRemovedFromCache(TimeSpan timeout)
            {
                return _removedFromCacheLock.Wait(timeout);
            }
        }

        #endregion

        private readonly List<int> _items;
        private readonly MemoryCache _pageCache;
        private readonly Dictionary<int, PageLifetime> _pageLifetimes;

        public VirtualCollectionImplementationSpy(IEnumerable<int> items, int pageSize) : base(pageSize)
        {
            _items = new List<int>(items);
            _pageCache = new MemoryCache(SynchronizationContext);
            _pageCache.CacheValueExpired += HandleCacheValueRemovedFromCache;
            _pageLifetimes = new Dictionary<int, PageLifetime>();
        }        

        protected override QueryCache PageCache
        {
            get { return _pageCache; }
        }

        public bool FailNextPageLoad
        {
            get;
            set;
        }

        public int LoadCountInvocations
        {
            get;
            private set;
        }

        public int GetFromCacheInvocations
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
            PageLifetime lifetime;

            if (_pageLifetimes.TryGetValue(pageIndex, out lifetime))
            {
                return lifetime.WaitUntilRemovedFromCache(timeout);
            }
            return false;
        }

        private void HandleCacheValueRemovedFromCache(object sender, QueryCacheValueExpiredEventArgs e)
        {
            int pageIndex = (int) e.Key;
            PageLifetime lifetime;

            if (_pageLifetimes.TryGetValue(pageIndex, out lifetime))
            {
                lifetime.NotifyRemovedFromCache();
            }
        }

        protected override QueryCacheValueLifetime CreatePageLifetime(int pageIndex, IList<int> page)
        {
            var lifetime = new PageLifetime(TimeSpan.FromMilliseconds(100));

            _pageLifetimes[pageIndex] = lifetime;

            return lifetime;
        }

        protected override bool TryGetPageFromCache(int pageIndex, out IList<int> page)
        {
            GetFromCacheInvocations++;

            return base.TryGetPageFromCache(pageIndex, out page);
        }

        protected override Task<VirtualCollectionPage<int>> StartLoadPageTask(int pageIndex)
        {
            LoadPageInvocations++;

            if (FailNextPageLoad)
            {
                return CreateFailedTask();
            }           
            return CreateCompletedTask(LoadPage(pageIndex));
        }

        private VirtualCollectionPage<int> LoadPage(int pageIndex)
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

        private static Task<VirtualCollectionPage<int>> CreateFailedTask()
        {
            using (var handle = new ManualResetEventSlim(false, 1000))
            {
                var task = Task<VirtualCollectionPage<int>>.Factory.StartNew(Fail);

                task.ContinueWith(t => handle.Set(), TaskContinuationOptions.ExecuteSynchronously);

                handle.Wait();

                return task;
            }
        }

        private static VirtualCollectionPage<int> Fail()
        {
            throw new Exception("Failed to load page.");
        }
    }
}
