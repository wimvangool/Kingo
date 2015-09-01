using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Client.DataVirtualization
{
    internal sealed class VirtualCollectionSpy : VirtualCollection<int>
    {
        #region [====== PageLifetime ======]

        private sealed class CachedPagePolicy : CacheItemPolicy
        {
            private readonly ManualResetEventSlim _removedFromCacheLock;

            public CachedPagePolicy(TimeSpan timeout)
            {
                _removedFromCacheLock = new ManualResetEventSlim();

                AbsoluteExpiration = Clock.Current.LocalDateAndTime().Add(timeout);
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

        private readonly ManualResetEventSlim _collectionRaisedEvent;
        private readonly AutoResetEvent _pageLoadWaitEvent; 
        private readonly List<int> _items;        
        private readonly Dictionary<int, CachedPagePolicy> _cachedPagePolicies;
        private readonly Lazy<VirtualCollectionPageLoader<int>> _loader;

        public VirtualCollectionSpy(IEnumerable<int> items, int pageSize)
            : base(pageSize, CollectionChangedEventThrottle.DefaultRaiseInterval, new SynchronousContext())
        {
            _collectionRaisedEvent = new ManualResetEventSlim();
            _pageLoadWaitEvent = new AutoResetEvent(false);
            _items = new List<int>(items);            
            _cachedPagePolicies = new Dictionary<int, CachedPagePolicy>();  
            _loader = new Lazy<VirtualCollectionPageLoader<int>>(GetLoader);
        }

        #region [====== Dispose ======]

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }            
            _pageLoadWaitEvent.Dispose();
            _collectionRaisedEvent.Dispose();
            
            base.Dispose(disposing);
        }

        #endregion

        #region [====== Count ======]

        public int LoadCountInvocations
        {
            get;
            private set;
        }

        protected internal override Task<int> StartLoadCountTask()
        {
            LoadCountInvocations++;
            
            return CreateCompletedTask(_items.Count);
        }

        #endregion

        #region [====== Page Loading ======]

        public int LoadPageInvocations
        {
            get;
            private set;
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

        internal VirtualCollectionPageLoader<int> Loader
        {
            get { return _loader.Value; }
        }

        private VirtualCollectionPageLoader<int> GetLoader()
        {
            return typeof(VirtualCollection<int>)
                .GetField("_loader", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(this) as VirtualCollectionPageLoader<int>;
        }

        public void SignalPageLoadToContinue()
        {
            _pageLoadWaitEvent.Set();
        }        

        protected internal override Task<IList<int>> StartLoadPageTask(int pageIndex)
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
            return new VirtualCollectionPage<int>(Loader, pageIndex, page);
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

        #endregion

        #region [====== Page Caching ======]

        public bool UseInfiniteCacheLifetime
        {
            get;
            set;
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

        protected internal override CacheItemPolicy CreatePageCachePolicy(int pageIndex, bool isErrorPage)
        {
            if (UseInfiniteCacheLifetime)
            {
                return null;
            }
            var policy = new CachedPagePolicy(TimeSpan.FromMilliseconds(100));

            _cachedPagePolicies[pageIndex] = policy;

            return policy;
        }

        #endregion

        #region [====== CollectionChanged ======]

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            _collectionRaisedEvent.Set();
        }

        internal bool WaitForCollectionChangedEvent(TimeSpan timeout)
        {
            return _collectionRaisedEvent.Wait(timeout);
        }

        #endregion
    }
}
