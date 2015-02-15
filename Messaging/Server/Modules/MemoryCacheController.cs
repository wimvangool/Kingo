using System.Runtime.Caching;
using System.Threading;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Represents a <see cref="ObjectCacheController" /> that is based on <see cref="MemoryCache" /> instances
    /// and associates sessions with the <see cref="Thread.CurrentPrincipal" />.
    /// </summary>
    public class MemoryCacheController : ObjectCacheController
    {
        private readonly Lazy<MemoryCache> _applicationCache;
        private readonly ReaderWriterLockSlim _applicationCacheLock;
        private readonly string _sessionCacheKeyFormat;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheController" /> class.
        /// </summary>        
        public MemoryCacheController()
        {
            _applicationCache = new Lazy<MemoryCache>(CreateApplicationCache, LazyThreadSafetyMode.ExecutionAndPublication);
            _applicationCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _sessionCacheKeyFormat = Guid.NewGuid().ToString("N") + "_{0}";            
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
                if (_applicationCache.IsValueCreated)
                {
                    _applicationCache.Value.Dispose();
                }
                _applicationCacheLock.Dispose();
            }
            base.Dispose(disposing);
        }

        #region [====== ApplicationCache ======]

        /// <summary>
        /// Returns the <see cref="MemoryCache"/> instance that serves as the application cache.
        /// </summary>
        protected MemoryCache ApplicationCache
        {
            get { return _applicationCache.Value; }
        }

        /// <inheritdoc />
        protected override bool TryGetApplicationCache(out ObjectCache cache)
        {
            cache = ApplicationCache;
            return true;
        }

        /// <summary>
        /// Creates and returns a <see cref="MemoryCache" /> instance that will serve as the application-cache.
        /// </summary>
        /// <returns>A <see cref="MemoryCache" /> instance that will serve as the application-cache.</returns>
        protected virtual MemoryCache CreateApplicationCache()
        {
            return new MemoryCache("ApplicationCache");
        }

        #endregion

        #region [====== SessionCache ======]

        /// <inheritdoc />
        protected override bool TryGetSessionCache(out ObjectCache cache)
        {
            string sessionKey;
            
            if (TryGetSessionKey(out sessionKey))
            {
                cache = GetOrAddSessionCache(sessionKey);
                return true;
            }
            cache = null;
            return false;
        }

        private ObjectCache GetOrAddSessionCache(string sessionKey)
        {            
            _applicationCacheLock.EnterReadLock();

            try
            {
                var cache = ApplicationCache.Get(sessionKey);
                if (cache != null)
                {
                    return (ObjectCache) cache;
                }
            }
            finally
            {
                _applicationCacheLock.ExitReadLock();
            }

            _applicationCacheLock.EnterUpgradeableReadLock();

            try
            {
                var cache = ApplicationCache.Get(sessionKey);
                if (cache != null)
                {
                    return (ObjectCache) cache;
                }
                var newCache = CreateSessionCache();                

                StoreSessionCache(sessionKey, newCache, CreateSessionCachePolicy());

                return newCache;
            }
            finally
            {
                _applicationCacheLock.ExitUpgradeableReadLock();
            }
        }

        private bool TryGetSessionKey(out string sessionKey)
        {
            var identity = Thread.CurrentPrincipal.Identity;
            if (identity.IsAuthenticated)
            {
                sessionKey = string.Format(_sessionCacheKeyFormat, identity.Name);
                return true;
            }
            sessionKey = null;
            return false;
        }

        /// <summary>
        /// Creates and returns a <see cref="ObjectCache" /> instance that will serve as the application-cache.
        /// </summary>
        /// <returns>A <see cref="ObjectCache" /> instance that will serve as the application-cache.</returns>
        protected virtual ObjectCache CreateSessionCache()
        {
            return new MemoryCache("SessionCache");
        }

        /// <summary>
        /// Creates and returns a <see cref="CacheItemPolicy" /> that is used to store a session-cache instance into
        /// the application cache instance.
        /// </summary>
        /// <returns>A <see cref="CacheItemPolicy" /> that is used to store a session-cache instance.</returns>
        /// <remarks>The default policy will maintain a sliding expiration of 10 minutes.</remarks>
        protected virtual CacheItemPolicy CreateSessionCachePolicy()
        {
            return new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
        }

        private void StoreSessionCache(string sessionKey, ObjectCache cache, CacheItemPolicy policy)
        {
            // Before we store the session cache, we add a callback that will dispose the cache
            // when it's expired/removed, if required.
            policy.RemovedCallback += args => OnSessionCacheExpired(cache);

            _applicationCacheLock.EnterWriteLock();

            try
            {
                ApplicationCache.Add(sessionKey, cache, policy);
            }
            finally
            {
                _applicationCacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Occurs when a session has expired and it's associated cache is destroyed.
        /// </summary>
        /// <param name="cache">The cache that was associated to the session.</param>
        protected virtual void OnSessionCacheExpired(ObjectCache cache)
        {
            var disposable = cache as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion
    }
}
