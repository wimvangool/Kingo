using System;

namespace YellowFlare.MessageProcessing.Messages
{
    /// <summary>
    /// Represents the result of query that is stored in a <see cref="QueryCache" />.
    /// </summary>
    /// <remarks>
    /// <see cref="QueryCacheValue">QueryCacheValues</see> are values or results that are associated with a
    /// certain <see cref="QueryCacheValueLifetime">lifetime</see> when they are added to the cache. When
    /// this lifetime expires, the value is automatically removed from the cache.
    /// </remarks>
    public sealed class QueryCacheValue : IDisposable
    {        
        private readonly object _value;
        private readonly QueryCacheValueLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheValue" /> class.
        /// </summary>
        /// <param name="value">The result of the query.</param>
        public QueryCacheValue(object value)
            : this(value, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheValue" /> class.
        /// </summary>
        /// <param name="value">The result of the query.</param>
        /// <param name="lifetime">The lifetime of this cached result.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="lifetime"/> is <c>null</c>.
        /// </exception>
        public QueryCacheValue(object value, QueryCacheValueLifetime lifetime)
        {            
            _value = value;
            _lifetime = lifetime ?? new InfiniteLifetime();
        }

        /// <summary>
        /// The lifetime of this value.
        /// </summary>
        public QueryCacheValueLifetime Lifetime
        {
            get { return _lifetime; }
        }

        /// <summary>
        /// Returns the cached value, casting it to the specified type, and notifies the associated lifetime
        /// that the value has been accessed.
        /// </summary>
        /// <typeparam name="TValue">Type to which the value is cast.</typeparam>
        /// <returns>The cached value.</returns>
        /// <exception cref="InvalidCastException">
        /// The stored value could not be cast the specified <paramtyperef name="TValue">type</paramtyperef>.
        /// </exception>
        public TValue Access<TValue>()
        {
            _lifetime.NotifyValueAccessed();

            return (TValue) _value;
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _lifetime.Dispose();
        }

        #endregion           
    }
}
