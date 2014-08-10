using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Contains extension methods for the <see cref="ObjectCache" /> class.
    /// </summary>
    public static class ObjectCacheExtensions
    {
        #region [====== TryGetValue ======]

        /// <summary>
        /// Attempts to retrieve a cached value from the cache.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to retrieve.</typeparam>
        /// <param name="cache">The cache to retrieve the value from.</param>
        /// <param name="key">The key of the cached value.</param>        
        /// <param name="value">
        /// If the method returns <c>true</c>, this parameter will contain the cached value after the method completes;
        /// will have the default value if this method returns <c>false</c>.
        /// </param>
        /// <returns><c>true</c> if the value was successfully retrieved from the cache; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> or <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TValue"/>.
        /// </exception>
        public static bool TryGetValue<TValue>(this ObjectCache cache, object key, out TValue value)
        {
            return TryGetValue(cache, key, null, out value);
        }

        /// <summary>
        /// Attempts to retrieve a cached value from the cache.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to retrieve.</typeparam>
        /// <param name="cache">The cache to retrieve the value from.</param>
        /// <param name="key">The key of the cached value.</param>
        /// <param name="regionName">Name of the region in the cache. Can be <c>null</c>, depending on the cache-implementation.</param>
        /// <param name="value">
        /// If the method returns <c>true</c>, this parameter will contain the cached value after the method completes;
        /// will have the default value if this method returns <c>false</c>.
        /// </param>
        /// <returns><c>true</c> if the value was successfully retrieved from the cache; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> or <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TValue"/>.
        /// </exception>
        public static bool TryGetValue<TValue>(this ObjectCache cache, object key, string regionName, out TValue value)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var cachedValue = cache.Get(key.ToString(), regionName);
            if (cachedValue == null)
            {
                value = default(TValue);
                return false;
            }
            value = (TValue)cachedValue;
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a cached value from the cache.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to retrieve.</typeparam>
        /// <param name="cache">The cache to retrieve the value from.</param>
        /// <param name="key">The key of the cached value.</param>        
        /// <param name="value">
        /// If the method returns <c>true</c>, this parameter will contain the cached value after the method completes;
        /// will have the default value if this method returns <c>false</c>.
        /// </param>
        /// <returns><c>true</c> if the value was successfully retrieved from the cache; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> or <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TValue"/>.
        /// </exception>
        public static bool TryGetValue<TValue>(this ObjectCache cache, string key, out TValue value)
        {
            return TryGetValue(cache, key, null, out value);
        }

        /// <summary>
        /// Attempts to retrieve a cached value from the cache.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to retrieve.</typeparam>
        /// <param name="cache">The cache to retrieve the value from.</param>
        /// <param name="key">The key of the cached value.</param>
        /// <param name="regionName">Name of the region in the cache. Can be <c>null</c>, depending on the cache-implementation.</param>
        /// <param name="value">
        /// If the method returns <c>true</c>, this parameter will contain the cached value after the method completes;
        /// will have the default value if this method returns <c>false</c>.
        /// </param>
        /// <returns><c>true</c> if the value was successfully retrieved from the cache; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> or <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TValue"/>.
        /// </exception>
        public static bool TryGetValue<TValue>(this ObjectCache cache, string key, string regionName, out TValue value)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            var cachedValue = cache.Get(key, regionName);
            if (cachedValue == null)
            {
                value = default(TValue);
                return false;
            }
            value = (TValue) cachedValue;
            return true;
        }

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Determines whether the specified <paramref name="cache"/> contains an entry for the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="cache">The cache to check in.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="regionName">Name of the region in the cache. Can be <c>null</c>, depending on the cache-implementation.</param>
        /// <returns><c>true</c> if an entry was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> or <paramref name="key"/> is <c>null</c>.
        /// </exception>
        public static bool Contains(this ObjectCache cache, object key, string regionName = null)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return cache.Contains(key.ToString(), regionName);
        }

        #endregion

        #region [====== Add ======]

        /// <summary>
        /// Inserts a cache entry into the cache, specifying information about how the entry will be evicted.
        /// </summary>
        /// <param name="cache">The cache to add the entry to.</param>
        /// <param name="key">The key of the value to cache.</param>
        /// <param name="value">The value to store in cache.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry.</param>
        /// <param name="regionName">Name of the region in the cache. Can be <c>null</c>, depending on the cache-implementation.</param>
        /// <returns>
        /// <c>true</c> if the insertion try succeeds, or <c>false</c> if there is an already an entry in the cache
        /// with the same key as <paramref name="key"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/>, <paramref name="key"/> or <paramref name="policy"/> is <c>null</c>.
        /// </exception>
        public static bool Add(this ObjectCache cache, object key, object value, CacheItemPolicy policy, string regionName = null)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return cache.Add(key.ToString(), value, policy, regionName);
        }

        #endregion

        #region [====== GetOrAdd ======]

        /// <summary>
        /// Attempts to retrieve a cached value, or if not found, adds a new value to the cache and returns that value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to store or retrieve.</typeparam>
        /// <param name="cache">The cache to retrieve the value from.</param>
        /// <param name="key">The key of the cached value.</param>
        /// <param name="policy">The policy that is used for storing the new value if no entry existed for the specified <paramref name="key"/>.</param>
        /// <param name="regionName">Name of the region in the cache. Can be <c>null</c>, depending on the cache-implementation.</param>
        /// <param name="valueFactory">
        /// The factory that is used to create a new value to store into the cache if no entry existed for the specified <paramref name="key"/>.
        /// </param>
        /// <returns>
        /// The cached value if it already existed in the cache, or the value created by <paramref name="valueFactory"/> if it was newly added.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/>, <paramref name="key"/>, <paramref name="valueFactory"/> or <paramref name="policy"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TValue"/>.
        /// </exception>
        public static TValue GetOrAdd<TValue>(this ObjectCache cache, object key, Func<TValue> valueFactory, CacheItemPolicy policy, string regionName = null)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to retrieve a cached value, or if not found, adds a new value to the cache and returns that value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to store or retrieve.</typeparam>
        /// <param name="cache">The cache to retrieve the value from.</param>
        /// <param name="key">The key of the cached value.</param>
        /// <param name="policy">The policy that is used for storing the new value if no entry existed for the specified <paramref name="key"/>.</param>
        /// <param name="regionName">Name of the region in the cache. Can be <c>null</c>, depending on the cache-implementation.</param>
        /// <param name="valueFactory">
        /// The factory that is used to create a new value to store into the cache if no entry existed for the specified <paramref name="key"/>.
        /// </param>
        /// <returns>
        /// The cached value if it already existed in the cache, or the value created by <paramref name="valueFactory"/> if it was newly added.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/>, <paramref name="key"/>, <paramref name="valueFactory"/> or <paramref name="policy"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TValue"/>.
        /// </exception>
        public static TValue GetOrAdd<TValue>(this ObjectCache cache, string key, Func<TValue> valueFactory, CacheItemPolicy policy, string regionName = null)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            throw new NotImplementedException();
        }

        #endregion
    }
}
