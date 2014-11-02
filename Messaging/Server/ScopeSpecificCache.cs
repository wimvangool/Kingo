using System.Collections.Generic;

namespace System.ComponentModel.Server
{   
    /// <summary>
    /// Provides a basic, in-memory implementation of the <see cref="IScopeSpecificCache" /> interface.
    /// </summary>
    public sealed class ScopeSpecificCache : IScopeSpecificCache, IDisposable
    {
        private readonly List<IDisposable> _entries;
        private bool _isDisposed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeSpecificCache" /> class.
        /// </summary>
        public ScopeSpecificCache()
        {
            _entries = new List<IDisposable>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            // We create a copy here since every Dispose()-action may result
            // in a call to Remove(), eventually resulting in the values-collection
            // being changed while iterating over it.
            var valuesCopy = new List<IDisposable>(_entries);

            foreach (var disposable in valuesCopy)
            {
                disposable.Dispose();
            }
            _isDisposed = true;
        }

        /// <inheritdoc />
        public IScopeSpecificCacheEntry<T> Add<T>(T value)
        {            
            return Add(value, null);
        }

        /// <inheritdoc />
        public IScopeSpecificCacheEntry<T> Add<T>(T value, Action<T> valueInvalidatedCallback)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            var cachedItem = new ScopeSpecificCacheEntry<T>(this, value, valueInvalidatedCallback);

            _entries.Add(cachedItem);

            return cachedItem;
        }   
   
        internal void Remove(IDisposable cachedItem)
        {
            _entries.Remove(cachedItem);
        }               

        private static Exception NewCacheDisposedException()
        {
            return new ObjectDisposedException(typeof(ScopeSpecificCache).Name);
        }        
    }
}
