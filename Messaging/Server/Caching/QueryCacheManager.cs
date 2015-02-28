using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Serves as a base-class for all <see cref="QueryCacheManager" />-instances. QueryCacheManagers are meant to
    /// manage a single cache-instance, either representing the ApplicationCache or a SessionCache, and provides
    /// synchronized adds, updates and removals of cache-entries through a <see cref="ReaderWriterLockSlim" />.
    /// </summary>
    public abstract class QueryCacheManager : IDisposable
    {
        private readonly ReaderWriterLockSlim _cacheLock;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheManager" /> class.
        /// </summary>
        /// <param name="recursionPolicy">
        /// The lock recursion policy that is used for the <see cref="CacheLock" />.
        /// </param>
        protected QueryCacheManager(LockRecursionPolicy recursionPolicy)
        {
            _cacheLock = new ReaderWriterLockSlim(recursionPolicy);
        }        

        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether or not this instance has been disposed.
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
                CacheLock.Dispose();
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
        
        #region [====== GetOrAdd ======]

        /// <summary>
        /// Returns the lock that is used to control read and write access to the cache.
        /// </summary>
        protected ReaderWriterLockSlim CacheLock
        {
            get { return _cacheLock; }
        }

        /// <summary>
        /// Returns the <see cref="QueryCacheModule" /> that created this manager.
        /// </summary>
        protected abstract QueryCacheModule CacheProvider
        {
            get;
        }

        internal TMessageOut GetOrAddToCache<TMessageOut>(IQuery<TMessageOut> query, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)            
            where TMessageOut : class, IMessage<TMessageOut>
        {            
            // First, an attempt is made to just read the result from cache, if allowed.            
            if (query.AllowCacheRead)
            {
                CacheLock.EnterReadLock();

                try
                {
                    object cachedResult;

                    if (TryGetCacheEntry(query.MessageIn, out cachedResult))
                    {
                        return ((TMessageOut) cachedResult).Copy();
                    }
                }
                finally
                {
                    CacheLock.ExitReadLock();
                }
            }

            // If the cached value was not found, we try again, but this time with an upgradeable lock,
            // because we may want to upgrade to a write to manupulate the cache.
            // Note that since we released the read lock, another request may have just added our wanted
            // result into the cache.
            CacheLock.EnterUpgradeableReadLock();

            try
            {
                // If a cache-read is allowed, and a cached result was found, we can return the cached value.
                // Otherwise, we have to remove or update the cache later.
                object cachedMessageOut;

                if (TryGetCacheEntry(query.MessageIn, out cachedMessageOut) && query.AllowCacheRead)
                {
                    return ((TMessageOut) cachedMessageOut).Copy();
                }

                // Since no cached value could be returned, we execute the query.
                var messageOut = query.Invoke();

                // If we are allowed to store the result into cache, we update the cache.
                // Otherwise, we remove any existing entry, since it has become stale.
                if (query.AllowCacheWrite && cachedMessageOut == null)
                {
                    InsertCacheEntryPostCommit(query.MessageIn.Copy(), messageOut.Copy(), absoluteExpiration, slidingExpiration);
                }
                else if (query.AllowCacheWrite)
                {
                    UpdateCacheEntryPostCommit(query.MessageIn, messageOut.Copy(), absoluteExpiration, slidingExpiration);
                }
                else if (cachedMessageOut != null)
                {
                    DeleteCacheEntryPostCommit(query.MessageIn);
                }
                return messageOut;
            }
            finally
            {
                CacheLock.ExitUpgradeableReadLock();
            } 
        }

        /// <summary>
        /// Attempts to retrieve a stored value from the cache.
        /// </summary>
        /// <param name="messageIn">The key of the cache-entry.</param>
        /// <param name="messageOut">The value of the cache-entry.</param>
        /// <returns><c>true</c> if the cache-entry exists; otherwise <c>false</c>.</returns>        
        protected abstract bool TryGetCacheEntry(object messageIn, out object messageOut);

        /// <summary>
        /// Determines if a cache-entry exists for the specified <paramref name="messageIn"/>.
        /// </summary>
        /// <param name="messageIn">The key of the cache-entry.</param>
        /// <returns><c>true</c> if the cache-entry exists; otherwise <c>false</c>.</returns>
        protected abstract bool ContainsCacheEntry(object messageIn);

        #endregion

        #region [====== Insert ======]

        /// <summary>
        /// Occurs when a cache-entry is inserted into the cache.
        /// </summary>
        public event EventHandler<CacheEntryInsertedOrUpdatedEventArgs> CacheEntryInserted;

        private void InsertCacheEntryPostCommit(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            // The cache entry is only added if no transaction is active or if the active transaction commits.
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                CacheLock.EnterWriteLock();

                try
                {
                    // If this code is running after an active transaction has committed, we are no longer holding
                    // the upgradeable lock, so we have to check for an existing cache entry here again. If it exists,
                    // a previous request has already added the same result and we do nothing.
                    if (isPostCommit && ContainsCacheEntry(messageIn))
                    {
                        return;
                    }
                    InsertCacheEntry(messageIn, messageOut, absoluteExpiration, slidingExpiration);                    
                }
                finally
                {
                    CacheLock.ExitWriteLock();
                }                
                OnCacheEntryInserted(new CacheEntryInsertedOrUpdatedEventArgs(messageIn, messageOut));
            });
        }

        /// <summary>
        /// Inserts a new cache-entry into the cache. This method can assume to be invoked in write-mode.
        /// </summary>
        /// <param name="messageIn">The key of the cache-entry.</param>
        /// <param name="messageOut">The value of the cache-entry.</param>
        /// <param name="absoluteExpiration">Optional timeout value that causes the cached result to expire after a fixed amount of time.</param>
        /// <param name="slidingExpiration">Optional timeout value that causes the cached result to expire after a certain amount of unused time.</param>
        protected abstract void InsertCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration);

        /// <summary>
        /// Raises the <see cref="CacheEntryInserted" /> event.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        protected virtual void OnCacheEntryInserted(CacheEntryInsertedOrUpdatedEventArgs e)
        {
            CacheEntryInserted.Raise(this, e);
            CacheProvider.OnCacheEntryInserted(e);
        }

        #endregion

        #region [====== Update ======]

        /// <summary>
        /// Occurs when a cache-entry is updated in the cache.
        /// </summary>
        public event EventHandler<CacheEntryInsertedOrUpdatedEventArgs> CacheEntryUpdated;

        private void UpdateCacheEntryPostCommit(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            // The cache entry is only updated if no transaction is active or if the active transaction commits.
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                bool entryWasUpdated;

                CacheLock.EnterWriteLock();

                try
                {
                    entryWasUpdated = UpdateCacheEntry(messageIn, messageOut, absoluteExpiration, slidingExpiration);                    
                }
                finally
                {
                    CacheLock.ExitWriteLock();
                }
                if (entryWasUpdated)
                {
                    OnCacheEntryUpdated(new CacheEntryInsertedOrUpdatedEventArgs(messageIn, messageOut));
                }
            });
        }

        /// <summary>
        /// Updates a cache-entry in the cache, if it exists. This method can assume to be invoked in write-mode.
        /// </summary>
        /// <param name="messageIn">The key of the cache-entry.</param>
        /// <param name="messageOut">The value of the cache-entry.</param>
        /// <param name="absoluteExpiration">Optional timeout value that causes the cached result to expire after a fixed amount of time.</param>
        /// <param name="slidingExpiration">Optional timeout value that causes the cached result to expire after a certain amount of unused time.</param>
        /// <returns><c>true</c> if the cache-entry was updated; otherwise <c>false</c>.</returns>
        protected abstract bool UpdateCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration);

        /// <summary>
        /// Raises the <see cref="CacheEntryUpdated" /> event.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        protected virtual void OnCacheEntryUpdated(CacheEntryInsertedOrUpdatedEventArgs e)
        {
            CacheEntryUpdated.Raise(this, e);
            CacheProvider.OnCacheEntryUpdated(e);
        }

        #endregion

        #region [====== Delete ======]

        /// <summary>
        /// Occurs when a cache-entry is deleted from the cache.
        /// </summary>
        public event EventHandler<CacheEntryDeletedEventArgs> CacheEntryDeleted;

        internal void DeleteCacheEntryPostCommit(object messageIn, TimeSpan? timeout = null)
        {
            // The cache entry is only removed if no transaction is active or if the active transaction commits.
            MessageProcessor.InvokePostCommit(isPostCommit =>
            {
                // If a timeout has been specified, the method is invoked on a cleanup thread.
                // In that scenario, only an attempt is made to obtain the write-lock in a timely fashion.
                // If it's not obtained within the timeout, the manager is assumed to be busy and cleanup
                // is skipped. Otherwise, a timeout of -1 milliseconds specifies an indefinite timeout.
                var actualTimeout = timeout.HasValue ? timeout.Value : TimeSpan.FromMilliseconds(-1);
                bool entryWasDeleted = false;

                if (CacheLock.TryEnterWriteLock(actualTimeout))
                {
                    try
                    {
                        entryWasDeleted = DeleteCacheEntry(messageIn);
                    }
                    finally
                    {
                        CacheLock.ExitWriteLock();
                    }                    
                }
                if (entryWasDeleted)
                {
                    OnCacheEntryDeleted(new CacheEntryDeletedEventArgs(messageIn));
                }
            });
        }

        /// <summary>
        /// Deletes a cache-entry from the cache.
        /// </summary>
        /// <param name="messageIn">The key of the cache-entry.</param>
        /// <returns><c>true</c> if the cache-entry was deleted; otherwise <c>false</c>.</returns>
        protected abstract bool DeleteCacheEntry(object messageIn);

        /// <summary>
        /// Raises the <see cref="CacheEntryUpdated" /> event.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        protected virtual void OnCacheEntryDeleted(CacheEntryDeletedEventArgs e)
        {
            CacheEntryDeleted.Raise(this, e);
            CacheProvider.OnCacheEntryDeleted(e);
        }

        #endregion
             
        internal void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class
        {
            foreach (var cacheEntry in SelectCacheEntriesWithKeyOfTypeInReadMode(typeof(TMessageIn)))
            {
                if (mustInvalidate.Invoke((TMessageIn) cacheEntry.MessageIn))
                {
                    cacheEntry.InvalidatePostCommit();
                }
            }
        }    
    
        private IEnumerable<CacheEntry> SelectCacheEntriesWithKeyOfTypeInReadMode(Type keyType)
        {
            CacheLock.EnterReadLock();

            try
            {
                return SelectCacheEntriesWithKeyOfType(keyType).ToArray();
            }
            finally
            {
                CacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Selects and returns all cache-entries that have a key that is an instance of <paramref name="keyType"/>.
        /// </summary>
        /// <param name="keyType">The type that is matched against all keys of the cache-entries.</param>
        /// <returns>A collection of cache-entries.</returns>
        protected abstract IEnumerable<CacheEntry> SelectCacheEntriesWithKeyOfType(Type keyType);
    }
}
