using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// The event-arguments associated with the <see cref="IQueryCache.CacheValueExpired" /> event.
    /// </summary>
    public class QueryCacheValueExpiredEventArgs : EventArgs
    {
        /// <summary>
        /// The execution-parameter of the query that stored this value in the cache.
        /// </summary>
        public readonly object Key;

        /// <summary>
        /// The result that was stored in the cache.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheValueExpiredEventArgs" /> class.
        /// </summary>
        /// <param name="key">The execution-parameter of the query that stored this value in the cache.</param>
        /// <param name="value">The result that was stored in the cache.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <c>null</c>.
        /// </exception>
        public QueryCacheValueExpiredEventArgs(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            Key = key;
            Value = value;
        }
    }
}
