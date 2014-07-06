using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a cache in which the results of different queries can be stored.
    /// </summary>
    /// <remarks>
    /// All methods of this interface should be implemented in a thread-safe manner.
    /// </remarks>
    public interface IQueryCache
    {
        /// <summary>
        /// Occurs when a cached value has expired and was removed from cache.
        /// </summary>
        event EventHandler<QueryCacheValueExpiredEventArgs> CacheValueExpired;

        /// <summary>
        /// Attempts to read a cached result from the cache using the query-parameters.
        /// </summary>
        /// <param name="messageIn">The query-parameters.</param>
        /// <param name="value">
        /// If the cache contains a value associated with the specified query-parameters, this parameters will
        /// contain the stored value after the call to this method completed; otherwise, this value will be
        /// <c>null</c> afterwards.
        /// </param>
        /// <returns>
        /// <c>true</c> if a stored value was found; false otherwise.
        /// </returns>        
        bool TryGetValue(object messageIn, out QueryCacheValue value);

        /// <summary>
        /// Attempts to read a cached result from the cache or adds it to the cache through the specified factory.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the parameter used to find or create the cache value.</typeparam>
        /// <param name="messageIn">The parameter used to find or create the cache value.</param>
        /// <param name="valueFactory">
        /// The factory used to create the cache value if it was not found in the cache.
        /// </param>
        /// <returns>The cached value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageIn"/> or <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        QueryCacheValue GetOrAdd<TMessageIn>(TMessageIn messageIn, Func<TMessageIn, QueryCacheValue> valueFactory);

        /// <summary>
        /// Removes all values from the cache.
        /// </summary>
        void Clear();
    }
}
