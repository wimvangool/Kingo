using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using Timer = System.Timers.Timer;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheController" /> interface 
    /// </summary>
    public abstract class ObjectCacheController : QueryCacheController
    {
        private readonly ReaderWriterLockSlim _cacheManagerLock;
        private readonly Dictionary<ObjectCache, ObjectCacheManager> _cacheManagers;

        private readonly Timer _timer;
        private readonly HashSet<ObjectCache> _emptyCaches;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCacheController" /> class.
        /// </summary>
        protected ObjectCacheController()
        {
            _cacheManagerLock = new ReaderWriterLockSlim();
            _cacheManagers = new Dictionary<ObjectCache, ObjectCacheManager>();

            _timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);
            _timer.AutoReset = false;
            _timer.Elapsed += (s, e) => RemoveEmptyCacheManagers();
            _emptyCaches = new HashSet<ObjectCache>();
        }

        /// <summary>
        /// Gets or sets the timeout that is maintained to clean up caches once one is detected to be empty.
        /// </summary>
        protected TimeSpan EmptyCacheCleanupInterval
        {
            get { return TimeSpan.FromMilliseconds(_timer.Interval); }
            set { _timer.Interval = value.TotalMilliseconds; }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                _cacheManagerLock.Dispose();

                foreach (var cacheManager in _cacheManagers)
                {
                    cacheManager.Value.Dispose();
                }                
                _timer.Dispose();                
            }
            base.Dispose(disposing);
        }

        #region [====== Getting or Adding Cache Entries ======]

        /// <inheritdoc />
        public override TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }            
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (message.Parameters == null)
            {
                throw NewMissingMessageParametersException("message");
            }
            ObjectCache cache;

            if (TryGetApplicationCache(out cache))
            {
                return GetOrAddObjectCacheManager(cache).GetOrAddToCache(message, query);
            }
            return query.Execute(message.Parameters);
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
        public override TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }            
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (message.Parameters == null)
            {
                throw NewMissingMessageParametersException("message");
            }
            ObjectCache cache;

            if (TryGetSessionCache(out cache))
            {
                return GetOrAddObjectCacheManager(cache).GetOrAddToCache(message, query);
            }
            return query.Execute(message.Parameters);
        }

        private ObjectCacheManager GetOrAddObjectCacheManager(ObjectCache cache)
        {
            ObjectCacheManager cacheManager;

            // In a first attempt we try to retrieve an existing instance by just obtaining a read lock.
            _cacheManagerLock.EnterReadLock();

            try
            {
                if (_cacheManagers.TryGetValue(cache, out cacheManager))
                {
                    return cacheManager;
                }
            }
            finally
            {
                _cacheManagerLock.ExitReadLock();
            }

            // If not found in the first attempt, we try again, but this time with an upgradeable lock, so that
            // we can upgrade to a write lock if neccessary.
            _cacheManagerLock.EnterUpgradeableReadLock();
            
            try
            {
                if (_cacheManagers.TryGetValue(cache, out cacheManager))
                {
                    return cacheManager;
                }
                cacheManager = new ObjectCacheManager(this, cache);

                AddCacheInstance(cache, cacheManager);

                return cacheManager;
            }
            finally
            {
                _cacheManagerLock.ExitUpgradeableReadLock();
            }
        }

        private void AddCacheInstance(ObjectCache cache, ObjectCacheManager cacheManager)
        {
            _cacheManagerLock.EnterWriteLock();

            try
            {
                _cacheManagers.Add(cache, cacheManager);
            }
            finally
            {
                _cacheManagerLock.ExitWriteLock();
            }
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

        /// <summary>
        /// Creates and returns a new <see cref="CacheItemPolicy" /> that will be used to store a new <see cref="IQuery{T, S}" />-result
        /// into cache.
        /// </summary>
        /// <param name="absoluteExpiration">The configured absolute expiration for this policy.</param>
        /// <param name="slidingExpiration">The configured sliding expiration for this policy.</param>
        /// <returns>A new <see cref="CacheItemPolicy" />.</returns>
        protected internal virtual CacheItemPolicy CreateCacheItemPolicy(TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
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

        private static Exception NewMissingMessageParametersException(string paramName)
        {
            return new ArgumentException(ExceptionMessages.ObjectCacheController_MissingMessageParameters, paramName);
        }

        #endregion

        #region [====== Invalidation and Removal of Cache Entries ======]

        /// <inheritdoc />
        public override void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (mustInvalidate == null)
            {
                throw new ArgumentNullException("mustInvalidate");
            }
            foreach (var cacheManager in AllCacheInstances())
            {
                cacheManager.InvalidateIfRequired(mustInvalidate);
            }
        }   
     
        private IEnumerable<ObjectCacheManager> AllCacheInstances()
        {
            _cacheManagerLock.EnterReadLock();

            try
            {
                return _cacheManagers.Values.ToArray();
            }
            finally
            {
                _cacheManagerLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Occurs when the result of a query has been added to one of the caches.
        /// </summary>        
        /// <param name="messageIn">Message containing the parameters of the query.</param>
        /// <param name="messageOut">The (stored) result of the query.</param>       
        /// <param name="cache">The cache the result was originally stored in.</param>         
        protected internal virtual void OnCacheItemAdded(object messageIn, object messageOut, ObjectCache cache) { }

        /// <summary>
        /// Occurs when the result of a query has been evicted from one of the caches.
        /// </summary>        
        /// <param name="messageIn">Message containing the parameters of the query.</param>
        /// <param name="messageOut">The (evicted) result of the query.</param>   
        /// <param name="cache">The cache the result was originally stored in.</param>
        /// <param name="reason">The reason of why the result was evicted.</param>     
        protected internal virtual void OnCacheItemRemoved(object messageIn, object messageOut, ObjectCache cache, CacheEntryRemovedReason reason)
        {
            if (cache.GetCount() > 0L)
            {
                return;
            }
            lock (_emptyCaches)
            {
                // When this is the first cache to be added to the emptyCache-collection,
                // we enable the timer to run. The reason we do not immediately dispose
                // of the associated manager is that caches might drop to a count of zero
                // for only an instance before new items are added again, and we do not want
                // to block any threads every time one cache item is removed from any ObjectCache
                // instance.
                if (_emptyCaches.Add(cache) && _emptyCaches.Count == 1)
                {
                    _timer.Enabled = true;
                }
            }
        }

        private void RemoveEmptyCacheManagers()
        {            
            foreach (var emptyCache in AllEmptyCaches())
            {                
                _cacheManagerLock.EnterWriteLock();

                try
                {
                    _cacheManagers[emptyCache].Dispose();
                    _cacheManagers.Remove(emptyCache);
                }
                finally
                {
                    _cacheManagerLock.ExitWriteLock();
                }
            }
        }

        private IEnumerable<ObjectCache> AllEmptyCaches()
        {
            lock (_emptyCaches)
            {
                var caches = new ObjectCache[_emptyCaches.Count];

                _emptyCaches.CopyTo(caches);
                _emptyCaches.Clear();

                return caches.Where(cache => cache.GetCount() == 0L);
            }
        }

        #endregion
    }
}
