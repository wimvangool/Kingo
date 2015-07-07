using System;
using System.Collections.Generic;

namespace Syztem.ComponentModel.Server
{
    /// <summary>
    /// Represents a cache that can be used to store dependencies that have a lifetime that is bound to a
    /// specific <see cref="Scenario" />.
    /// </summary>
    internal sealed class ScenarioCache : DependencyCacheRelay
    {
        private readonly Dictionary<object, object> _cacheEntries;

        private ScenarioCache()
        {
            _cacheEntries = new Dictionary<object, object>();
        }

        internal TValue GetOrAdd<TValue>(object key, Func<TValue> valueFactory)
        {            
            lock (_cacheEntries)
            {
                IDependencyCacheEntry<TValue> cacheEntry;
                TValue value;

                if (TryGetCacheEntry(key, out cacheEntry) && cacheEntry.TryGetValue(out value))
                {
                    return value;
                }
                value = valueFactory.Invoke();

                _cacheEntries[key] = Add(value, v => OnValueRemoved(key));

                return value;
            }
        }

        private bool TryGetCacheEntry<TValue>(object key, out IDependencyCacheEntry<TValue> cacheEntry)
        {
            object value;

            if (_cacheEntries.TryGetValue(key, out value))
            {
                cacheEntry = value as IDependencyCacheEntry<TValue>;
            }
            else
            {
                cacheEntry = null;
            }
            return cacheEntry != null;
        }

        private void OnValueRemoved(object key)
        {
            lock (_cacheEntries)
            {
                _cacheEntries.Remove(key);
            }
        }

        /// <inheritdoc />
        protected override bool TryGetCache(out IDependencyCache cache)
        {
            return (cache = Scenario.Cache) != null;
        }

        internal static readonly ScenarioCache Instance = new ScenarioCache();        
    }
}
