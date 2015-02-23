using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheProvider" /> interface.
    /// </summary>
    public abstract class QueryCacheProvider : IQueryCacheProvider
    {
        private readonly ReaderWriterLockSlim _cacheManagerLock;
        private readonly Dictionary<object, QueryCacheManager> _cacheManagers;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheProvider" /> class.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The lock recursion policy that is used for the <see cref="CacheManagerLock" />.
        /// </param>
        protected QueryCacheProvider(LockRecursionPolicy recursionPolicy)
        {
            _cacheManagerLock = new ReaderWriterLockSlim(recursionPolicy);
            _cacheManagers = new Dictionary<object, QueryCacheManager>();
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether or not the current instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                CacheManagerLock.Dispose();
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ObjectDisposedException" />.
        /// </summary>
        /// <returns>A new <see cref="ObjectDisposedException" />.</returns>
        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region [====== Caching ======]

        /// <summary>
        /// Returns the lock that is used to control read and write access to the cache-managers.
        /// </summary>
        protected ReaderWriterLockSlim CacheManagerLock
        {
            get { return _cacheManagerLock; }
        }

        /// <inheritdoc /> 
        public TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryCachePolicy policy)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            IQueryCacheManagerFactory cacheManagerFactory;

            if (TryGetApplicationCacheFactory(out cacheManagerFactory))
            {
                return GetOrAddCacheManager(cacheManagerFactory).GetOrAddToCache(message, query, policy);
            }
            return query.Execute(message);
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
        public TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryCachePolicy policy)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            IQueryCacheManagerFactory cacheManagerFactory;

            if (TryGetSessionCacheFactory(out cacheManagerFactory))
            {
                return GetOrAddCacheManager(cacheManagerFactory).GetOrAddToCache(message, query, policy);
            }
            return query.Execute(message);
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
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (mustInvalidate == null)
            {
                throw new ArgumentNullException("mustInvalidate");
            }
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
