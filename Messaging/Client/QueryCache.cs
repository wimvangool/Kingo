using System.Diagnostics;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents the base-class for every cache that is used to store the results of queries.
    /// </summary>
    /// <remarks>
    /// Note to implementers: since the <see cref="QueryCache" /> is to be used in a concurrent environment,
    /// all instance methods of this class must be thread-safe.
    /// </remarks>
    public abstract class QueryCache : AsyncObject
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCache" /> class.
        /// </summary>
        protected QueryCache() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCache" /> class.
        /// </summary>
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        protected QueryCache(SynchronizationContext synchronizationContext) : base(synchronizationContext) { }
  
        /// <summary>
        /// Occurs when a cache value expired and was removed from cache.
        /// </summary>
        public event EventHandler<QueryCacheValueExpiredEventArgs> CacheValueExpired;

        /// <summary>
        /// Raises the <see cref="CacheValueExpired" /> event.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        protected virtual void OnCachedValueExpired(QueryCacheValueExpiredEventArgs e)
        {
            CacheValueExpired.Raise(this, e);
        }

        private void OnCachedValueExpired(object key)
        {
            QueryCacheValue cacheValue;

            if (TryRemove(key, out cacheValue))
            {
                var value = cacheValue.Access<object>();
                
                cacheValue.Dispose();

                using (var scope = CreateSynchronizationContextScope())
                {
                    scope.Post(() => OnCachedValueExpired(new QueryCacheValueExpiredEventArgs(key, value)));
                }
            }
        }

        /// <summary>
        /// When overridden, attempts to retrieve a value from the cache using the specified key.
        /// </summary>
        /// <param name="key">The key that is used to retrieve the value.</param>
        /// <param name="value">
        /// If the cache contains a value associated with the specified <paramref name="key"/>, this parameter will
        /// refer to this value after the method completed. Will be <c>null</c> otherwise.
        /// </param>
        /// <returns>
        /// <c>true</c> if the cache found an entry using the specified <paramref name="key"/>; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Note that values may expire <i>while</i> they are retrieved. This race-condition is unaviodable and may cause this method
        /// to return a value that has actually (just) expired when used.
        /// </remarks>
        public abstract bool TryGetValue(object key, out QueryCacheValue value);

        /// <summary>
        /// Attempts to add a new entry into the cache. If an entry with the specified key already exists in the cache,
        /// it is replaced by <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key that is used to store the value.</param>
        /// <param name="value">The value to store.</param>            
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public void AddOrUpdate<TKey>(TKey key, QueryCacheValue value)
        {
            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException("key");
            }            
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            AddOrUpdate(key, value, (k, oldValue) =>
            {
                oldValue.Dispose();

                return value;
            });
            RegisterNew(key, value);
        }

        /// <summary>
        /// When overridden, attempts to add a new entry into the cache. If an entry with the specified key already exists in the cache,
        /// <paramref name="updateValueFactory"/> is used to update the value.
        /// </summary>
        /// <param name="key">The key that is used to store the value.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="updateValueFactory">
        /// The factory used to update a value in the cache, if another entry with the specified <paramref name="key"/> was already present in
        /// the cache.
        /// </param>                        
        protected abstract void AddOrUpdate(object key, QueryCacheValue value, Func<object, QueryCacheValue, QueryCacheValue> updateValueFactory);

        /// <summary>
        /// Attempts to retrieve a value from cache, but if not found, creates and stores it before it is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="key">The key that is used to retrieve or store the value.</param>
        /// <param name="valueFactory">
        /// Factory method that is used to create the value if it is not already present in the cache.
        /// </param>
        /// <returns>
        /// The value that was either stored or created.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> or <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Note that values may expire <i>while</i> they are retrieved. This race-condition is unaviodable and may cause this method
        /// to return a value that has actually (just) expired when used.
        /// </remarks>
        public QueryCacheValue GetOrAdd<TKey>(TKey key, Func<TKey, QueryCacheValue> valueFactory)
        {
            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException("key");
            }
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            // If the value was retrieved from cache, it can immediately be returned.
            QueryCacheValue value;

            if (TryGetOrAdd(key, valueFactory, out value))
            {
                return value;
            }
            RegisterNew(key, value);
           
            return value;
        }  
      
        private void RegisterNew(object key, QueryCacheValue value)
        {
            // If the value was created and stored, its Expired-event is hooked onto a handler that will
            // remove the value as soon as it expires.
            value.Lifetime.Expired += (s, e) => OnCachedValueExpired(key);
            value.Lifetime.Start();            
        }

        /// <summary>
        /// Attempts to retrieve a value from cache, but if not found, creates and stores it before it is returned.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="key">The key that is used to retrieve or store the value.</param>
        /// <param name="valueFactory">
        /// Factory method that is used to create the value if it is not already present in the cache.
        /// </param>
        /// <param name="value">
        /// After the method completes, will contain the value that was either retrieved or created.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was retrieved from cache; <c>false</c> if it was created and stored.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The value returned by <paramref name="valueFactory"/> was <c>null</c>.
        /// </exception>
        protected abstract bool TryGetOrAdd<TKey>(TKey key, Func<TKey, QueryCacheValue> valueFactory, out QueryCacheValue value);

        /// <summary>
        /// When overridden, attempts to remove a value from the cache using the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The key that is used to remove the value.
        /// </param>
        /// <param name="value">
        /// If the value was removed, this parameter will contain the removed value; will otherwise be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the value was removed; otherwise <c>false</c>.</returns>
        public abstract bool TryRemove(object key, out QueryCacheValue value);

        /// <summary>
        /// When overriden, removes all values from the cache.
        /// </summary>        
        public abstract void Clear();

        /// <summary>
        /// Attempts to read a result from the cache.
        /// </summary>
        /// <typeparam name="TResult">Type of the result to retrieve.</typeparam>
        /// <param name="cache">The cache to read the result from (optional).</param>
        /// <param name="key">The key used to read the result.</param>
        /// <param name="result">
        /// If <paramref name="cache"/> is not <c>null</c> and the cache contains the
        /// value stored by the specified <paramref name="key"/>, this parameter will contain
        /// the stored result.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was successfully read from the cache; otherwise <c>false</c>.
        /// </returns>
        public static bool TryGetFromCache<TResult>(QueryCache cache, object key, out TResult result)
        {
            QueryCacheValue value;

            if (cache != null && cache.TryGetValue(key, out value))
            {
                result = value.Access<TResult>();
                return true;
            }
            result = default(TResult);
            return false;
        }       
    }
}
