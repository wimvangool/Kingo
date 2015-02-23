using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheProvider" /> interface 
    /// </summary>
    public abstract class ObjectCacheProvider : QueryCacheProvider
    {
        #region [====== ObjectCacheManagerFactory ======]

        private sealed class ObjectCacheManagerFactory : QueryCacheManagerFactory<ObjectCache>
        {
            private readonly ObjectCacheProvider _cacheProvider;

            internal ObjectCacheManagerFactory(ObjectCache cache, ObjectCacheProvider cacheProvider) : base(cache)
            {
                _cacheProvider = cacheProvider;
            }

            public override QueryCacheManager CreateCacheManager()
            {
                return new ObjectCacheManager(Cache, _cacheProvider);
            }
        }

        #endregion        

        #region [====== Getting or Adding Cache Entries ======]

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCacheProvider" /> class.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The lock recursion policy that is used for the <see cref="QueryCacheProvider.CacheManagerLock" />.
        /// </param>
        protected ObjectCacheProvider(LockRecursionPolicy recursionPolicy)
            : base(recursionPolicy) { }

        /// <inheritdoc />
        protected override bool TryGetApplicationCacheFactory(out IQueryCacheManagerFactory applicationCacheManagerFactory)
        {
            ObjectCache applicationCache;

            if (TryGetApplicationCache(out applicationCache))
            {
                applicationCacheManagerFactory = new ObjectCacheManagerFactory(applicationCache, this);
                return true;
            }
            applicationCacheManagerFactory = null;
            return false;
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
        protected override bool TryGetSessionCacheFactory(out IQueryCacheManagerFactory sessionCacheManagerFactory)
        {
            ObjectCache sessionCache;

            if (TryGetSessionCache(out sessionCache))
            {
                sessionCacheManagerFactory = new ObjectCacheManagerFactory(sessionCache, this);
                return true;
            }
            sessionCacheManagerFactory = null;
            return false;
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

        #endregion

        /// <summary>
        /// Creates and returns a set of <see cref="ChangeMonitor"/> instances that are used to
        /// observe changes in the system that would invalidate the cache-entry that is stored
        /// for <paramref name="messageIn"/>.
        /// </summary>
        /// <param name="messageIn">The key of the cache-entry being stored.</param>
        /// <returns><see cref="ChangeMonitor"/> instances.</returns>
        protected internal virtual IEnumerable<ChangeMonitor> CreateChangeMonitors(object messageIn)
        {
            return Enumerable.Empty<ChangeMonitor>();
        }        
    }
}
