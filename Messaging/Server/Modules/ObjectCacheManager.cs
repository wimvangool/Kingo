using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace System.ComponentModel.Server.Modules
{
    internal sealed class ObjectCacheManager : IDisposable
    {
        private readonly ObjectCacheController _cacheManager;
        private readonly ObjectCache _cache;

        private readonly ReaderWriterLockSlim _cacheLock;
        private readonly Dictionary<object, QueryCacheEntryMonitor> _cacheEntryMonitors;        
        private bool _isDisposed;
        
        internal ObjectCacheManager(ObjectCacheController cacheManager, ObjectCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cacheManager = cacheManager;
            _cache = cache;

            _cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _cacheEntryMonitors = new Dictionary<object, QueryCacheEntryMonitor>();
        }        

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            // Note that we deliberately do not Dispose of the ObjectCache itself,
            // since that's the responsibility of the component that created that instance.
            // Also note that because of early disposal of ObjectCacheManagers, 
            // multiple Managers may be created for a single ObjectCache-instance during
            // the lifetime of the application.
            _cacheLock.Dispose();

            foreach (var monitor in _cacheEntryMonitors.Values)
            {
                monitor.Dispose();
            }            
            _isDisposed = true;
        }

        internal TMessageOut GetOrAddToCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            // First, an attempt is made to just read the result from cache, if allowed.
            var messageIn = message.Parameters;
            if (message.AllowCacheRead)
            {                
                _cacheLock.EnterReadLock();

                try
                {
                    object cachedResult;

                    if (TryGetCachedValue(messageIn, out cachedResult))
                    {
                        return ((TMessageOut) cachedResult).Copy();
                    }
                }
                finally
                {
                    _cacheLock.ExitReadLock();
                }
            }

            // If the cached value was not found, we try again, but this time with an upgradeable lock,
            // because we may want to upgrade to a write to manupulate the cache.

            // Note that since we released the read lock, another request may have just added our wanted
            // result into the cache.
            _cacheLock.EnterUpgradeableReadLock();

            try
            {
                // If a cache-read is allowed, and a cached result was found, we can return the cached value.
                // Otherwise, we have to remove or update the cache later.
                object cachedMessageOut;

                if (TryGetCachedValue(messageIn, out cachedMessageOut) && message.AllowCacheRead)
                {
                    return ((TMessageOut) cachedMessageOut).Copy();
                }

                // Since no cached value could be returned, we execute the query.
                var messageOut = query.Execute(messageIn);

                // If we are allowed to store the result into cache, we update the cache.
                // Otherwise, we remove any existing entry, since it has become stale.
                if (message.AllowCacheWrite && cachedMessageOut == null)
                {
                    InsertCacheEntry(messageIn.Copy(), messageOut.Copy(), message.AbsoluteExpiration, message.SlidingExpiration);
                }
                else if (message.AllowCacheWrite)
                {
                    UpdateCacheEntry(messageIn, messageOut.Copy(), message.AbsoluteExpiration, message.SlidingExpiration);
                }
                else if (cachedMessageOut != null)
                {
                    DeleteCacheEntry(messageIn);
                }
                return messageOut;
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }           
        }

        private bool TryGetCachedValue(object messageIn, out object messageOut)
        {
            QueryCacheEntryMonitor changeMonitor;

            if (_cacheEntryMonitors.TryGetValue(messageIn, out changeMonitor))
            {
                messageOut = _cache.Get(changeMonitor.UniqueId);
                return messageOut != null;
            }
            messageOut = null;
            return false;
        }

        private void InsertCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            // The cache entry is only added if no transaction is active or if the active transaction commits.
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                _cacheLock.EnterWriteLock();

                try
                {
                    // If this code is running after an active transaction has committed, we are no longer holding
                    // the upgradeable lock, so we have to check for an existing cache entry here again. If it exists,
                    // a previous request has already added the same result and we do nothing.
                    if (isPostCommit && _cacheEntryMonitors.ContainsKey(messageIn))
                    {
                        return;
                    }
                    var changeMonitor = new QueryCacheEntryMonitor();
                    var policy = CreateCacheItemPolicy(messageIn, absoluteExpiration, slidingExpiration, changeMonitor);

                    _cache.Add(changeMonitor.UniqueId, messageOut, policy);
                    _cacheEntryMonitors.Add(messageIn, changeMonitor);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
                _cacheManager.OnCacheItemAdded(messageIn, messageOut, _cache);
            });
        }

        private void UpdateCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            // The cache entry is only updated if no transaction is active or if the active transaction commits.
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                _cacheLock.EnterWriteLock();

                try
                {
                    QueryCacheEntryMonitor monitor;

                    if (_cacheEntryMonitors.TryGetValue(messageIn, out monitor))
                    {
                        _cache.Set(monitor.UniqueId, messageOut, CreateCacheItemPolicy(messageIn, absoluteExpiration, slidingExpiration, monitor.Copy()));
                    }
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
            });
        }

        private void DeleteCacheEntry(object messageIn)
        {
            // The cache entry is only removed if no transaction is active or if the active transaction commits.
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                _cacheLock.EnterWriteLock();

                try
                {
                    QueryCacheEntryMonitor monitor;

                    if (_cacheEntryMonitors.TryGetValue(messageIn, out monitor))
                    {
                        _cacheEntryMonitors.Remove(messageIn);
                        _cache.Remove(monitor.UniqueId);
                    }                                        
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }               
            });
        }

        private CacheItemPolicy CreateCacheItemPolicy(object messageIn, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, ChangeMonitor changeMonitor)
        {
            var policy = _cacheManager.CreateCacheItemPolicy(absoluteExpiration, slidingExpiration);

            policy.ChangeMonitors.Add(changeMonitor);
            policy.RemovedCallback += args =>
            {
                _cacheLock.EnterWriteLock();

                try
                {
                    // If a cache item has been removed, we have to remove the corresponding
                    // item from the cacheEntryMonitors as well.
                    _cacheEntryMonitors.Remove(messageIn);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }
                _cacheManager.OnCacheItemRemoved(messageIn, args.CacheItem.Value, args.Source, args.RemovedReason);
            };
            return policy;
        }        

        internal void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class
        {
            var monitorsToNotify = new List<QueryCacheEntryMonitor>();

            foreach (var cacheItem in AllChangeMonitorsFor<TMessageIn>())
            {
                if (mustInvalidate.Invoke(cacheItem.Key))
                {
                    monitorsToNotify.Add(cacheItem.Value);
                }
            }
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                foreach (var monitor in monitorsToNotify)
                {
                    monitor.RemoveCacheEntry();
                }
            });
        }

        private IEnumerable<KeyValuePair<TMessageIn, QueryCacheEntryMonitor>> AllChangeMonitorsFor<TMessageIn>() where TMessageIn : class
        {
            _cacheLock.EnterReadLock();

            try
            {
                var monitors =
                    from pair in _cacheEntryMonitors
                    let messageIn = pair.Key as TMessageIn
                    where messageIn != null
                    select new KeyValuePair<TMessageIn, QueryCacheEntryMonitor>(messageIn, pair.Value);

                return monitors.ToArray();
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }
    }
}
