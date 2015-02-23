using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace System.ComponentModel.Server.Caching
{
    internal sealed class ObjectCacheManager : QueryCacheManager
    {
        private readonly ObjectCacheProvider _cacheProvider;
        private readonly ObjectCache _cache;       
        private readonly Dictionary<object, ObjectCacheEntryMonitor> _cacheEntryMonitors;                
        
        internal ObjectCacheManager(ObjectCache cache, ObjectCacheProvider cacheProvider) : base(LockRecursionPolicy.NoRecursion)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cacheProvider = cacheProvider;
            _cache = cache;            
            _cacheEntryMonitors = new Dictionary<object, ObjectCacheEntryMonitor>();
        }               

        protected override QueryCacheProvider CacheProvider
        {
            get { return _cacheProvider; }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                // Note that we deliberately do not Dispose of the ObjectCache itself,
                // since that's the responsibility of the component that created that instance.
                // Also note that because of early disposal of ObjectCacheManagers, 
                // multiple Managers may be created for a single ObjectCache-instance during
                // the lifetime of the application.
                foreach (var monitor in _cacheEntryMonitors.Values)
                {
                    monitor.Dispose();
                } 
            }
            base.Dispose(disposing);
        }
        
        protected override bool TryGetCacheEntry(object messageIn, out object messageOut)
        {
            ObjectCacheEntryMonitor changeMonitor;

            if (_cacheEntryMonitors.TryGetValue(messageIn, out changeMonitor))
            {
                messageOut = _cache.Get(changeMonitor.UniqueId);
                return messageOut != null;
            }
            messageOut = null;
            return false;
        }
        
        protected override bool ContainsCacheEntry(object messageIn)
        {
            return _cacheEntryMonitors.ContainsKey(messageIn);
        }
        
        protected override void InsertCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            var changeMonitor = new ObjectCacheEntryMonitor();
            var policy = CreateCacheItemPolicy(messageIn, absoluteExpiration, slidingExpiration, changeMonitor);

            _cache.Add(changeMonitor.UniqueId, messageOut, policy);
            _cacheEntryMonitors.Add(messageIn, changeMonitor);            

            OnCacheEntryInserted(new CacheEntryInsertedOrUpdatedEventArgs(messageIn, messageOut));
        }
        
        protected override bool UpdateCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            ObjectCacheEntryMonitor monitor;

            if (_cacheEntryMonitors.TryGetValue(messageIn, out monitor))
            {
                _cache.Set(monitor.UniqueId, messageOut, CreateCacheItemPolicy(messageIn, absoluteExpiration, slidingExpiration, monitor.Copy()));

                return true;
            }
            return false;
        }
        
        protected override bool DeleteCacheEntry(object messageIn)
        {
            ObjectCacheEntryMonitor monitor;

            if (_cacheEntryMonitors.TryGetValue(messageIn, out monitor))
            {
                _cacheEntryMonitors.Remove(messageIn);
                _cache.Remove(monitor.UniqueId);

                return true;
            }
            return false;
        }

        private CacheItemPolicy CreateCacheItemPolicy(object messageIn, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, ChangeMonitor monitor)
        {
            var firstMonitor =  new [] { monitor };
            var otherMonitors = _cacheProvider.CreateChangeMonitors(messageIn);
            var allMonitors = firstMonitor.Concat(otherMonitors);

            return CreateCacheItemPolicy(messageIn, absoluteExpiration, slidingExpiration, allMonitors);
        }

        private CacheItemPolicy CreateCacheItemPolicy(object messageIn, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IEnumerable<ChangeMonitor> monitors)
        {
            var policy = new CacheItemPolicy();

            if (absoluteExpiration.HasValue)
            {
                // NB: We explicitly use DateTimeOffSet.Now since the built-in cache always refers
                // to the physical Date and Time of the current machine. There is no way we could
                // mock out this behavior by using Clock.Current().
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(absoluteExpiration.Value);
            }
            if (slidingExpiration.HasValue)
            {
                policy.SlidingExpiration = slidingExpiration.Value;
            }
            foreach (var changeMonitor in monitors)
            {
                policy.ChangeMonitors.Add(changeMonitor);    
            }            
            policy.RemovedCallback += args =>
            {
                bool entryWasDeleted;

                CacheLock.EnterWriteLock();

                try
                {
                    // If a cache item has been removed, we have to remove the corresponding
                    // item from the cacheEntryMonitors as well.
                    entryWasDeleted = _cacheEntryMonitors.Remove(messageIn);
                }
                finally
                {
                    CacheLock.ExitWriteLock();
                }
                if (entryWasDeleted)
                {
                    OnCacheEntryDeleted(new CacheEntryDeletedEventArgs(messageIn));
                }
            };
            return policy;
        }

        protected override IEnumerable<CacheEntry> SelectCacheEntriesWithKeyOfType(Type keyType)
        {
            return
                from pair in _cacheEntryMonitors
                where keyType.IsInstanceOfType(pair.Key)
                select new ObjectCacheEntry(pair.Key, pair.Value);
        }        
    }
}
