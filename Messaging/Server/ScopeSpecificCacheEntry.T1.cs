namespace System.ComponentModel.Messaging.Server
{
    internal sealed class ScopeSpecificCacheEntry<T> : IScopeSpecificCacheEntry<T>
    {
        private readonly ScopeSpecificCache _cache;
        private readonly T _value;
        private readonly Action<T> _valueInvalidatedCallback;        
        private bool _isDisposed;        

        public ScopeSpecificCacheEntry(ScopeSpecificCache cache, T value, Action<T> valueInvalidatedCallback)
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
            RemoveValue();

            _isDisposed = true;
        }

        private void RemoveValue()
        {
            if (_valueInvalidatedCallback != null)
            {
                _valueInvalidatedCallback.Invoke(_value);
            }
            _cache.Remove(this);
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
