using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a cache that relays to a another cache-implementation. This class can be useful
    /// when a component wants to refer to a cache of which it's availability is context-dependent.
    /// </summary>
    public abstract class CacheRelay : ICache
    {
        /// <inheritdoc />
        public ICacheEntry<T> Add<T>(T value)
        {
            return CurrentCache().Add(value);
        }

        /// <inheritdoc />
        public ICacheEntry<T> Add<T>(T value, Action<T> valueRemovedCallback)
        {
            return CurrentCache().Add(value, valueRemovedCallback);
        }

        private ICache CurrentCache()
        {
            ICache cache;

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
        protected abstract bool TryGetCache(out ICache cache);

        
        private static InvalidOperationException NewCacheNotAvailableException()
        {
            return new InvalidOperationException(ExceptionMessages.CacheRelay_NotAvailable);
        }
    }
}
