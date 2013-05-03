using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YellowFlare.MessageProcessing
{   
    internal sealed class MessageProcessorContextCache : IDictionary<Guid, object>, IDisposable
    {
        private readonly IDictionary<Guid, object> _cache;
        private bool _isDisposed;
        
        public MessageProcessorContextCache()
        {
            _cache = new Dictionary<Guid, object>();
        }
        
        public void Dispose()
        {
            foreach (var disposable in _cache.Values.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
            _isDisposed = true;
        }

        public int Count
        {
            get { return _cache.Count; }
        }

        public bool IsReadOnly
        {
            get { return _cache.IsReadOnly; }
        }

        public ICollection<Guid> Keys
        {
            get { return _cache.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _cache.Values; }
        }

        public object this[Guid key]
        {
            get { return _cache[key]; }
            set
            {
                if (_isDisposed)
                {
                    throw NewCacheDisposedException();
                }
                _cache[key] = value;
            }
        }

        public bool ContainsKey(Guid key)
        {
            return _cache.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<Guid, object> item)
        {
            return _cache.Contains(item);
        }

        public bool TryGetValue(Guid key, out object value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Add(Guid key, object value)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            _cache.Add(key, value);
        }

        public void Add(KeyValuePair<Guid, object> item)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            _cache.Add(item);
        }

        public bool Remove(Guid key)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            return _cache.Remove(key);
        }

        public bool Remove(KeyValuePair<Guid, object> item)
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            return _cache.Remove(item);
        }

        public void Clear()
        {
            if (_isDisposed)
            {
                throw NewCacheDisposedException();
            }
            _cache.Clear();
        }

        public void CopyTo(KeyValuePair<Guid, object>[] array, int arrayIndex)
        {
            _cache.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Guid, object>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }        

        private Exception NewCacheDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }
    }
}
