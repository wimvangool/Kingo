using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheModule" /> interface.
    /// </summary>
    public abstract class QueryCacheModule : QueryModule<QueryCacheOptionsAttribute>, IQueryCacheModule
    {
        private readonly ReaderWriterLockSlim _cacheManagerLock;
        private readonly Dictionary<object, QueryCacheManager> _cacheManagers;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheModule" /> class.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The lock recursion policy that is used for the <see cref="CacheManagerLock" />.
        /// </param>
        protected QueryCacheModule(LockRecursionPolicy recursionPolicy)
        {
            _cacheManagerLock = new ReaderWriterLockSlim(recursionPolicy);
            _cacheManagers = new Dictionary<object, QueryCacheManager>();
        }

        #region [====== Dispose ======]        

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (DisposeLock.IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                CacheManagerLock.Dispose();
            }
            base.Dispose(disposing);
        }        

        #endregion

        /// <inheritdoc />
        protected override TMessageOut InvokeQuery<TMessageOut>(IQuery<TMessageOut> query, IEnumerable<QueryCacheOptionsAttribute> attributes)
        {
            var attribute = attributes.SingleOrDefault();
            if (attribute == null)
            {
                return query.Invoke();
            }
            return attribute.GetOrAddToCache(query, this);
        }

        #region [====== Caching ======]

        /// <summary>
        /// Returns the lock that is used to control read and write access to the cache-managers.
        /// </summary>
        protected ReaderWriterLockSlim CacheManagerLock
        {
            get { return _cacheManagerLock; }
        }

        internal TMessageOut GetOrAddToApplicationCache<TMessageOut>(IQuery<TMessageOut> query, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)            
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            IQueryCacheManagerFactory cacheManagerFactory;

            if (TryGetApplicationCacheFactory(out cacheManagerFactory))
            {
                return GetOrAddCacheManager(cacheManagerFactory).GetOrAddToCache(query, absoluteExpiration, slidingExpiration);
            }
            return query.Invoke();
        }

        /// <summary>
        /// Attempts to retrieve the <see cref="IQueryCacheManagerFactory" /> for the application cache.
        /// This methods returns <c>false</c> if the application cache is not available.
        /// </summary>
        /// <param name="applicationCacheManagerFactory">
        /// When this method returns <c>true</c>, refers to the <see cref="IQueryCacheManagerFactory" /> that
        /// is used to create a new <see cref="ObjectCacheManager" /> for the application cache.
        /// </param>
        /// <returns><c>true</c> if the application cache is available; otherwise <c>false</c>.</returns>
        protected abstract bool TryGetApplicationCacheFactory(out IQueryCacheManagerFactory applicationCacheManagerFactory);

        /// <inheritdoc />       
        public TMessageOut GetOrAddToSessionCache<TMessageOut>(IQuery<TMessageOut> query, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)            
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            IQueryCacheManagerFactory cacheManagerFactory;

            if (TryGetSessionCacheFactory(out cacheManagerFactory))
            {
                return GetOrAddCacheManager(cacheManagerFactory).GetOrAddToCache(query, absoluteExpiration, slidingExpiration);
            }
            return query.Invoke();
        }

        /// <summary>
        /// Attempts to retrieve the <see cref="IQueryCacheManagerFactory" /> for the session cache.
        /// This methods returns <c>false</c> if the session cache is not available.
        /// </summary>
        /// <param name="sessionCacheManagerFactory">
        /// When this method returns <c>true</c>, refers to the <see cref="IQueryCacheManagerFactory" /> that
        /// is used to create a new <see cref="ObjectCacheManager" /> for the session cache.
        /// </param>
        /// <returns><c>true</c> if the session cache is available; otherwise <c>false</c>.</returns>
        protected abstract bool TryGetSessionCacheFactory(out IQueryCacheManagerFactory sessionCacheManagerFactory);

        private QueryCacheManager GetOrAddCacheManager(IQueryCacheManagerFactory cacheManagerFactory)
        {
            QueryCacheManager cacheManager;

            // In a first attempt we try to retrieve an existing instance by just obtaining a read lock.
            CacheManagerLock.EnterReadLock();

            try
            {
                if (_cacheManagers.TryGetValue(cacheManagerFactory.Cache, out cacheManager))
                {
                    return cacheManager;
                }
            }
            finally
            {
                CacheManagerLock.ExitReadLock();
            }

            // If not found in the first attempt, we try again, but this time with an upgradeable lock, so that
            // we can upgrade to a write lock if neccessary.
            CacheManagerLock.EnterUpgradeableReadLock();

            try
            {
                if (_cacheManagers.TryGetValue(cacheManagerFactory.Cache, out cacheManager))
                {
                    return cacheManager;
                }
                cacheManager = cacheManagerFactory.CreateCacheManager();

                StoreManager(cacheManagerFactory.Cache, cacheManager);

                return cacheManager;
            }
            finally
            {
                CacheManagerLock.ExitUpgradeableReadLock();
            }
        }

        private void StoreManager(object cache, QueryCacheManager cacheManager)
        {
            CacheManagerLock.EnterWriteLock();

            try
            {
                _cacheManagers.Add(cache, cacheManager);
            }
            finally
            {
                CacheManagerLock.ExitWriteLock();
            }
        }

        /// <inheritdoc />      
        public void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class
        {            
            if (mustInvalidate == null)
            {
                throw new ArgumentNullException("mustInvalidate");
            }
            DisposeLock.EnterMethod();

            try
            {
                InvalidateIfRequiredCore(mustInvalidate);
            }
            finally
            {
                DisposeLock.ExitMethod();
            }
        }

        private void InvalidateIfRequiredCore<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class
        {
            CacheManagerLock.EnterReadLock();

            try
            {
                foreach (var cacheManager in _cacheManagers.Values)
                {
                    cacheManager.InvalidateIfRequired(mustInvalidate);
                }
            }
            finally
            {
                CacheManagerLock.ExitReadLock();
            }
        }

        #endregion

        #region [====== Events ======]

        /// <summary>
        /// This method is invoked when a cache-entry has been inserted into one the caches managed by a <see cref="QueryCacheManager" />.
        /// </summary>        
        /// <param name="e">The argument of the event.</param>
        protected internal virtual void OnCacheEntryInserted(CacheEntryInsertedOrUpdatedEventArgs e)
        {
            Debug.WriteLine("CacheEntry Inserted: [{0}, {1}]", e.MessageIn.GetType().Name, e.MessageOut.GetType().Name);
        }

        /// <summary>
        /// This method is invoked when a cache-entry has been updated in one the caches managed by a <see cref="QueryCacheManager" />.
        /// </summary>        
        /// <param name="e">The argument of the event.</param>
        protected internal virtual void OnCacheEntryUpdated(CacheEntryInsertedOrUpdatedEventArgs e)
        {
            Debug.WriteLine("CacheEntry Updated: [{0}, {1}]", e.MessageIn.GetType().Name, e.MessageOut.GetType().Name);
        }

        /// <summary>
        /// This method is invoked when a cache-entry has been deleted from one the caches managed by a <see cref="QueryCacheManager" />.
        /// </summary>        
        /// <param name="e">The argument of the event.</param>
        protected internal virtual void OnCacheEntryDeleted(CacheEntryDeletedEventArgs e)
        {
            Debug.WriteLine("CacheEntry Deleted from: [{0}]", e.MessageIn.GetType().Name);
        }

        /// <summary>
        /// This method is invoked when a cache has expired and can be removed from memory.
        /// </summary>        
        protected internal virtual void OnCacheExpired(CacheExpiredEventArgs e)
        {
            CacheManagerLock.EnterWriteLock();

            try
            {
                QueryCacheManager cacheManager;

                if (_cacheManagers.TryGetValue(e.Cache, out cacheManager) && _cacheManagers.Remove(e.Cache))
                {
                    cacheManager.Dispose();
                }
            }
            finally
            {
                CacheManagerLock.ExitWriteLock();
            }
        }

        #endregion
    }
}
