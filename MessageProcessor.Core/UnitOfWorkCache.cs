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
            foreach (var disposable in _values)
            {
                disposable.Dispose();
            }
            _isDisposed = true;
        }

        public ICachedItem<T> Add<T>(T value)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            var cachedItem = new CachedItem<T>(value);

            _values.Add(cachedItem);

            return cachedItem;
        }

        public ICachedItem<T> Add<T>(T value, Action<T> valueRemovedCallback)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            var cachedItem = new CachedItem<T>(value, valueRemovedCallback);

            _values.Add(cachedItem);

            return cachedItem;
        }      

        private static Exception NewCacheDisposedException()
        {
            return new ObjectDisposedException(typeof(UnitOfWorkCache).Name);
        }        
    }
}
