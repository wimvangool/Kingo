using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Transactions;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheManager" /> interface 
    /// </summary>
    public abstract class ObjectCacheManager : QueryCacheManager
    {
        private readonly ReaderWriterLockSlim _cacheLock;
        private readonly Dictionary<object, QueryCacheEntryMonitor> _cacheEntryMonitors;

        protected ObjectCacheManager()
        {
            _cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _cacheEntryMonitors = new Dictionary<object, QueryCacheEntryMonitor>();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                _cacheLock.Dispose();

                foreach (var monitor in _cacheEntryMonitors.Values)
                {
                    monitor.Dispose();
                }
                _cacheEntryMonitors.Clear();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
        {            
            ObjectCache cache;

            if (TryGetApplicationCache(out cache))
            {
                return GetOrAddToCache(message, absoluteExpiration, slidingExpiration, query, cache);
            }
            return query.Execute(message);
        }

        /// <summary>
        /// Attempts to retrieve the <see cref="ObjectCache" /> that is used as the application cache.
        /// </summary>
        /// <param name="cache">
        /// If the application cache is available in the current context, this parameter will refer to the
        /// application after this method returns; otherwise, it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> is the application cache is available; otherwise <c>false</c>.</returns>
        protected abstract bool TryGetApplicationCache(out ObjectCache cache);

        /// <inheritdoc />
        protected override TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
        {            
            ObjectCache cache;

            if (TryGetSessionCache(out cache))
            {
                return GetOrAddToCache(message, absoluteExpiration, slidingExpiration, query, cache);
            }
            return query.Execute(message);
        }

        /// <summary>
        /// Attempts to retrieve the <see cref="ObjectCache" /> that is used as the session cache.
        /// </summary>
        /// <param name="cache">
        /// If the session cache is available in the current context, this parameter will refer to the
        /// application after this method returns; otherwise, it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> is the application cache is available; otherwise <c>false</c>.</returns>
        protected abstract bool TryGetSessionCache(out ObjectCache cache);

        private TMessageOut GetOrAddToCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query, ObjectCache cache)
            where TMessageIn : class, IMessage<TMessageIn>
        {
            // First, an attempt is made to just read the result from cache.
            _cacheLock.EnterReadLock();

            try
            {
                object cachedResult;

                if (TryGetCachedValue(cache, message, out cachedResult))
                {
                    return (TMessageOut) cachedResult;
                }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
            
            // If the cached value was not found, we try again, but this time with an upgradeable lock.
            // Note that since we released the read lock, another request may have just added our wanted
            // result into the cache.
            _cacheLock.EnterUpgradeableReadLock();

            try
            {
                object cachedResult;

                if (TryGetCachedValue(cache, message, out cachedResult))
                {
                    return (TMessageOut) cachedResult;
                }
                var messageOut = query.Execute(message);
                
                AddToCache(cache, absoluteExpiration, slidingExpiration, message.Copy(), messageOut);

                return messageOut;
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }                                            
        }

        private bool TryGetCachedValue(ObjectCache cache, object messageIn, out object messageOut)
        {
            QueryCacheEntryMonitor changeMonitor;

            if (_cacheEntryMonitors.TryGetValue(messageIn, out changeMonitor))
            {
                messageOut = cache.Get(changeMonitor.UniqueId);
                return messageOut != null;
            }
            messageOut = null;
            return false;
        }

        private void AddToCache(ObjectCache cache, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, object messageIn, object messageOut)
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
                    var changeMonitor = CreateQueryCacheEntryMonitor();
                    var policy = CreateCacheItemPolicy(messageIn, absoluteExpiration, slidingExpiration, changeMonitor);

                    cache.Add(changeMonitor.UniqueId, messageOut, policy);

                    _cacheEntryMonitors.Add(messageIn, changeMonitor);
                }
                finally
                {
                    _cacheLock.ExitWriteLock();
                }    
                OnCacheItemAdded(messageIn, messageOut, cache);
            });            
        }

        private CacheItemPolicy CreateCacheItemPolicy(object messageIn, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, ChangeMonitor changeMonitor)
        {
            var policy = CreateCacheItemPolicy(absoluteExpiration, slidingExpiration);
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
                OnCacheItemRemoved(messageIn, args.CacheItem.Value, args.Source, args.RemovedReason);
            };
            return policy;
        }        

        /// <summary>
        /// Creates and returns a new <see cref="CacheItemPolicy" /> that will be used to store a new <see cref="IQuery{T, S}" />-result
        /// into cache.
        /// </summary>
        /// <param name="absoluteExpiration">The configured absolute expiration for this policy.</param>
        /// <param name="slidingExpiration">The configured sliding expiration for this policy.</param>
        /// <returns>A new <see cref="CacheItemPolicy" />.</returns>
        protected virtual CacheItemPolicy CreateCacheItemPolicy(TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
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
            return policy;
        }

        private static QueryCacheEntryMonitor CreateQueryCacheEntryMonitor()
        {
            // A change monitor is created that is immediately invalidated when
            // any active transaction is not completed succesfully.
            var changeMonitor = new QueryCacheEntryMonitor();

            var transaction = Transaction.Current;
            if (transaction != null)
            {
                transaction.TransactionCompleted += (s, e) =>
                {
                    if (e.Transaction.TransactionInformation.Status != TransactionStatus.Committed)
                    {
                        changeMonitor.RemoveCacheEntry();
                    }
                };
            }
            return changeMonitor;
        }

        /// <inheritdoc />
        protected override void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate)
        {
            var monitorsToNotify = new List<QueryCacheEntryMonitor>();

            foreach (var cacheItem in ObtainChangeMonitorsFor<TMessageIn>())
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

        private IEnumerable<KeyValuePair<TMessageIn, QueryCacheEntryMonitor>> ObtainChangeMonitorsFor<TMessageIn>() where TMessageIn : class
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

        /// <summary>
        /// Occurs when the result of a query has been added to one of the caches.
        /// </summary>        
        /// <param name="messageIn">Message containing the parameters of the query.</param>
        /// <param name="messageOut">The (stored) result of the query.</param>       
        /// <param name="cache">The cache the result was originally stored in.</param>         
        protected virtual void OnCacheItemAdded(object messageIn, object messageOut, ObjectCache cache) { }

        /// <summary>
        /// Occurs when the result of a query has been evicted from one of the caches.
        /// </summary>        
        /// <param name="messageIn">Message containing the parameters of the query.</param>
        /// <param name="messageOut">The (evicted) result of the query.</param>   
        /// <param name="cache">The cache the result was originally stored in.</param>
        /// <param name="reason">The reason of why the result was evicted.</param>     
        protected virtual void OnCacheItemRemoved(object messageIn, object messageOut, ObjectCache cache, CacheEntryRemovedReason reason) { }
    }
}
