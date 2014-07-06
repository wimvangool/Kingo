using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents an in-memory, thread-safe implementation of the <see cref="IQueryCache" />-interface.
    /// </summary>
    public class QueryCache : IQueryCache
    {
        private readonly ConcurrentDictionary<object, QueryCacheValue> _cachedValues;
        private readonly IDispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCache" /> class.
        /// </summary>
        /// <param name="dispatcher">
        /// Dispatcher required to raise the <see cref="CacheValueExpired" />-event.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public QueryCache(IDispatcher dispatcher)
            : this(dispatcher, new ConcurrentDictionary<object, QueryCacheValue>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCache" /> class.
        /// </summary>
        /// <param name="dispatcher">
        /// Dispatcher required to raise the <see cref="CacheValueExpired" />-event.
        /// </param>
        /// <param name="comparer">
        /// The equality comparison implementation to use when comparing keys.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> or <paramref name="comparer"/> is <c>null</c>.
        /// </exception>
        public QueryCache(IDispatcher dispatcher, IEqualityComparer<object> comparer)
            : this(dispatcher, new ConcurrentDictionary<object, QueryCacheValue>(comparer)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCache" /> class.
        /// </summary>
        /// <param name="dispatcher">
        /// Dispatcher required to raise the <see cref="CacheValueExpired" />-event.
        /// </param>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the <see cref="QueryCache" /> concurrently.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="QueryCache" /> can contain.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel"/> is less than 1 or <paramref name="capacity"/> is less than 0.
        /// </exception>
        public QueryCache(IDispatcher dispatcher, int concurrencyLevel, int capacity)
            : this(dispatcher, new ConcurrentDictionary<object, QueryCacheValue>(concurrencyLevel, capacity)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCache" /> class.
        /// </summary>
        /// <param name="dispatcher">
        /// Dispatcher required to raise the <see cref="CacheValueExpired" />-event.
        /// </param>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the <see cref="QueryCache" /> concurrently.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="QueryCache" /> can contain.
        /// </param>
        /// <param name="comparer">
        /// The equality comparison implementation to use when comparing keys.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> or <paramref name="comparer"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel"/> is less than 1 or <paramref name="capacity"/> is less than 0.
        /// </exception>
        public QueryCache(IDispatcher dispatcher, int concurrencyLevel, int capacity, IEqualityComparer<object> comparer)
            : this(dispatcher, new ConcurrentDictionary<object, QueryCacheValue>(concurrencyLevel, capacity, comparer)) { }

        private QueryCache(IDispatcher dispatcher, ConcurrentDictionary<object, QueryCacheValue> cachedValues)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            _cachedValues = cachedValues;
            _dispatcher = dispatcher;
        }

        /// <inheritdoc />
        public event EventHandler<QueryCacheValueExpiredEventArgs> CacheValueExpired;

        /// <summary>
        /// Raises the <see cref="CacheValueExpired" /> event.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        protected virtual void OnCachedValueExpired(QueryCacheValueExpiredEventArgs e)
        {
            CacheValueExpired.Raise(this, e);
        }

        /// <inheritdoc />
        public virtual bool TryGetValue(object messageIn, out QueryCacheValue value)
        {
            return _cachedValues.TryGetValue(messageIn, out value);
        }

        /// <inheritdoc />
        public virtual QueryCacheValue GetOrAdd<TMessageIn>(TMessageIn messageIn, Func<TMessageIn, QueryCacheValue> valueFactory)
        {
            return _cachedValues.GetOrAdd(messageIn, key =>
            {
                var cachedValue = valueFactory.Invoke(messageIn);

                cachedValue.Expired += (s, e) =>
                {
                    QueryCacheValue removedValue;

                    if (_cachedValues.TryRemove(messageIn, out removedValue))
                    {
                        var value = removedValue.Access<object>();

                        _dispatcher.Invoke(() => OnCachedValueExpired(new QueryCacheValueExpiredEventArgs(messageIn, value)));

                        removedValue.Dispose();
                    }
                };
                return cachedValue;
            });
        }

        /// <inheritdoc />
        public virtual void Clear()
        {
            foreach (var key in _cachedValues.Keys)
            {
                QueryCacheValue removedValue;

                if (_cachedValues.TryRemove(key, out removedValue))
                {
                    removedValue.Dispose();
                }
            }
        }                
    }
}
