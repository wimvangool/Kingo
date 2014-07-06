using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{   
    internal sealed class UnitOfWorkCache : ICache, IDisposable
    {
        private readonly List<IDisposable> _values;
        private bool _isDisposed;
        
        public UnitOfWorkCache()
        {
            _values = new List<IDisposable>();
        }
        
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            // We create a copy here since every Dispose()-action may result
            // in a call to Remove(), eventually resulting in the values-collection
            // being changed while iterating over it.
            var valuesCopy = new List<IDisposable>(_values);

            foreach (var disposable in valuesCopy)
            {
                disposable.Dispose();
            }
            _isDisposed = true;
        }

        public ICachedItem<T> Add<T>(T value)
        {            
            return Add(value, null);
        }

        public ICachedItem<T> Add<T>(T value, Action<T> valueInvalidatedCallback)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            var cachedItem = new CachedItem<T>(this, value, valueInvalidatedCallback);

            _values.Add(cachedItem);

            return cachedItem;
        }   
   
        internal void Remove(IDisposable cachedItem)
        {
            _values.Remove(cachedItem);
        }

        private static Exception NewCacheDisposedException()
        {
            return new ObjectDisposedException(typeof(UnitOfWorkCache).Name);
        }        
    }
}
