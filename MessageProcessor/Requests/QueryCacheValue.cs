using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents the result of query that is stored in a <see cref="IQueryCache" />.
    /// </summary>
    /// <remarks>
    /// <see cref="QueryCacheValue">QueryCacheValues</see> are values or results that are associated with a
    /// certain <see cref="IQueryCacheValueLifetime">lifetime</see> when they are added to the cache. When
    /// this lifetime expires, the value is automatically removed from the cache.
    /// </remarks>
    public sealed class QueryCacheValue : IDisposable
    {        
        private readonly object _value;
        private readonly IQueryCacheValueLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheValue" /> class.
        /// </summary>
        /// <param name="value">The result of the query.</param>
        /// <param name="lifetime">The lifetime of this cached result.</param>
        /// <remarks>
        /// If <paramref name="lifetime"/> is <c>null</c>, this value will remain in cache
        /// indefinitely.
        /// </remarks>
        public QueryCacheValue(object value, IQueryCacheValueLifetime lifetime)
        {
            _value = value;
            _lifetime = lifetime;
        }

        /// <summary>
        /// Indicates whether or not thisvalue has expired.
        /// </summary>
        public bool IsExpired
        {
            get { return _lifetime != null && _lifetime.IsExpired; }
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
            if (_lifetime != null)
            {
                _lifetime.NotifyValueAccessed();
            }
            return (TValue)_value;
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_lifetime != null)
            {
                _lifetime.Dispose();
            }
        }

        #endregion

        #region [====== Lifetime Management ======]

        /// <summary>
        /// Occurs when the lifetime of this value has expired.
        /// </summary>
        public event EventHandler Expired
        {
            add
            {
                if (_lifetime != null)
                {
                    _lifetime.Expired += value;
                }
            }
            remove
            {
                if (_lifetime != null)
                {
                    _lifetime.Expired -= value;
                }
            }
        }

        #endregion

        #region [====== InfiniteLifetime ======]

        /// <summary>
        /// Creates and returns a new <see cref="QueryCacheValue" /> that has an infinite lifetime.
        /// </summary>
        /// <param name="value">The value to store in cache.</param>
        /// <returns>A new <see cref="QueryCacheValue" /> that has an infinite lifetime</returns>
        public static QueryCacheValue WithInfiniteLifetime(object value)
        {
            return new QueryCacheValue(value, null);
        }

        #endregion
    }
}
