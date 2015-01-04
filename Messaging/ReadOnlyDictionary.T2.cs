using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Diagnostics;
using System.Text;

namespace System.ComponentModel
{
    [Serializable]
    internal sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IDictionary<TKey, TValue> _dictionary;

        internal ReadOnlyDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        internal ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? new Dictionary<TKey, TValue>();
        }

        public override string ToString()
        {
            var errors = new StringBuilder();

            foreach (var error in _dictionary)
            {
                errors.AppendFormat("[{0} -> {1}] ", error.Key, error.Value);
            }
            return errors.ToString();
        }

        #region [====== IDictionary<TKey,TValue> ======]

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw NewReadOnlyException();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw NewReadOnlyException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return this[key]; }
            set { throw NewReadOnlyException(); }
        }

        #endregion

        #region [====== ICollection<KeyValuePair<TKey,TValue>> ======]

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw NewReadOnlyException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw NewReadOnlyException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw NewReadOnlyException();
        }

        #endregion

        #region [====== IEnumerable ======]

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static Exception NewReadOnlyException()
        {
            return new NotSupportedException(ExceptionMessages.ReadOnlyDictionary_NotSupported);
        }
    }
}
