using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class CachedItem<T> : ICachedItem<T>, IDisposable
    {
        private readonly T _value;
        private readonly Action<T> _valueRemovedCallback;
        private bool _isDisposed;

        public CachedItem(T value)
        {
            _value = value;            
        }

        public CachedItem(T value, Action<T> valueRemovedCallback)
        {
            _value = value;
            _valueRemovedCallback = valueRemovedCallback;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            if (_valueRemovedCallback != null)
            {
                _valueRemovedCallback.Invoke(_value);
            }
            _isDisposed = true;
        }

        public bool TryGetValue(out T value)
        {
            if (_isDisposed)
            {
                value = default(T);
                return false;
            }
            value = _value;
            return true;
        }
    }
}
