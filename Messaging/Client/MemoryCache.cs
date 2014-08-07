using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Resources;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="QueryCache" /> that caches all results in RAM.
    /// </summary>
    public class MemoryCache : QueryCache
    {
        private readonly ConcurrentDictionary<object, QueryCacheValue> _cachedValues;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache" /> class.
        /// </summary>        
        public MemoryCache()
        {
            _cachedValues = new ConcurrentDictionary<object, QueryCacheValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache" /> class.
        /// </summary>
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        public MemoryCache(SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
            _cachedValues = new ConcurrentDictionary<object, QueryCacheValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache" /> class.
        /// </summary>   
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>     
        /// <param name="comparer">
        /// The equality comparison implementation to use when comparing keys. Uses the default comparer if this
        /// parameter is <c>null</c>.
        /// </param>        
        public MemoryCache(SynchronizationContext synchronizationContext, IEqualityComparer<object> comparer) : base(synchronizationContext)
        {
            _cachedValues = new ConcurrentDictionary<object, QueryCacheValue>(comparer ?? EqualityComparer<object>.Default);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache" /> class.
        /// </summary> 
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>       
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the <see cref="MemoryCache" /> concurrently.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="MemoryCache" /> can contain.
        /// </param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel"/> is less than 1 or <paramref name="capacity"/> is less than 0.
        /// </exception>
        public MemoryCache(SynchronizationContext synchronizationContext, int concurrencyLevel, int capacity) : base(synchronizationContext)
        {
            _cachedValues = new ConcurrentDictionary<object, QueryCacheValue>(concurrencyLevel, capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCache" /> class.
        /// </summary>        
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the <see cref="MemoryCache" /> concurrently.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="MemoryCache" /> can contain.
        /// </param>
        /// <param name="comparer">
        /// The equality comparison implementation to use when comparing keys.
        /// </param>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel"/> is less than 1 or <paramref name="capacity"/> is less than 0.
        /// </exception>
        public MemoryCache(SynchronizationContext synchronizationContext, int concurrencyLevel, int capacity, IEqualityComparer<object> comparer)
            : base(synchronizationContext)
        {
            _cachedValues = new ConcurrentDictionary<object, QueryCacheValue>(concurrencyLevel, capacity, comparer ?? EqualityComparer<object>.Default);
        }        

        /// <inheritdoc />
        public override bool TryGetValue(object messageIn, out QueryCacheValue value)
        {
            return _cachedValues.TryGetValue(messageIn, out value);
        }

        /// <inheritdoc />
        protected override void AddOrUpdate(object key, QueryCacheValue value, Func<object, QueryCacheValue, QueryCacheValue> updateValueFactory)
        {
            _cachedValues.AddOrUpdate(key, value, updateValueFactory);
        }

        /// <inheritdoc />
        protected override bool TryGetOrAdd<TKey>(TKey key, Func<TKey, QueryCacheValue> valueFactory, out QueryCacheValue value)
        {
            bool wasRetrievedFromCache = true;

            value = _cachedValues.GetOrAdd(key, keyAlias =>
            {
                wasRetrievedFromCache = false;

                var newValue = valueFactory.Invoke(key);
                if (newValue == null)
                {
                    throw NewNullValueCreatedException("valueFactory");
                }
                return newValue;
            });
            return wasRetrievedFromCache;
        }

        private static Exception NewNullValueCreatedException(string paramName)
        {
            return new ArgumentException(ExceptionMessages.MemoryCache_NullValueCreated, paramName);
        }

        /// <inheritdoc />
        public override bool TryRemove(object key, out QueryCacheValue value)
        {
            return _cachedValues.TryRemove(key, out value);
        }

        /// <inheritdoc />
        public override void Clear()
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
