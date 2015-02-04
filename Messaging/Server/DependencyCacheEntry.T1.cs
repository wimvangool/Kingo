using System.Diagnostics;

namespace System.ComponentModel.Server
{
    internal sealed class DependencyCacheEntry<T> : IDependencCacheEntry<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DependencyCache _cache;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Action<T> _valueInvalidatedCallback;  

        private readonly T _value;              
        private bool _isDisposed;        

        public DependencyCacheEntry(DependencyCache cache, T value, Action<T> valueInvalidatedCallback)
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

        public override string ToString()
        {
            var value = _value as object;
            if (value == null)
            {
                return "null";
            }
            return value.ToString();
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
