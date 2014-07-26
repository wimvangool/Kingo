using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a cache that relays to a another cache-implementation. This class can be useful
    /// when a component wants to refer to a cache of which it's availability is context-dependent.
    /// </summary>
    public abstract class ScopeSpecificCacheRelay : IScopeSpecificCache
    {
        /// <inheritdoc />
        public IScopeSpecificCacheEntry<T> Add<T>(T value)
        {
            return CurrentCache().Add(value);
        }

        /// <inheritdoc />
        public IScopeSpecificCacheEntry<T> Add<T>(T value, Action<T> valueRemovedCallback)
        {
            return CurrentCache().Add(value, valueRemovedCallback);
        }

        private IScopeSpecificCache CurrentCache()
        {
            IScopeSpecificCache cache;

            if (TryGetCache(out cache))
            {
                return cache;
            }
            throw NewCacheNotAvailableException();
        }

        /// <summary>
        /// Attempts to retrieve another cache-implementation.
        /// </summary>
        /// <param name="cache">
        /// When the cache is avalailable, this parameter will refer to the cache retrieved.
        /// </param>
        /// <returns><c>true</c> is the cache was available; otherwise <c>false</c>.</returns>
        protected abstract bool TryGetCache(out IScopeSpecificCache cache);

        
        private static InvalidOperationException NewCacheNotAvailableException()
        {
            return new InvalidOperationException(ExceptionMessages.CacheRelay_NotAvailable);
        }
    }
}
