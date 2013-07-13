using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class CachedItem<T> : ICachedItem<T>, IDisposable
    {
        private readonly UnitOfWorkCache _cache;
        private readonly T _value;
        private readonly Action<T> _valueInvalidatedCallback;
        private bool _hasBeenInvalidated;
        private bool _isDisposed;        

        public CachedItem(UnitOfWorkCache cache, T value, Action<T> valueInvalidatedCallback)
        {            
            _cache = cache;
            _value = value;
            _valueInvalidatedCallback = valueInvalidatedCallback;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            Invalidate();

            _isDisposed = true;
        }

        public bool TryGetValue(out T value)
        {
            if (_hasBeenInvalidated)
            {
                value = default(T);
                return false;
            }
            value = _value;
            return true;
        }

        public void Invalidate()
        {
            if (_hasBeenInvalidated)
            {
                return;
            }
            if (_valueInvalidatedCallback != null)
            {
                _valueInvalidatedCallback.Invoke(_value);
            }
            _cache.Remove(this);
            _hasBeenInvalidated = true;
        }
    }
}
