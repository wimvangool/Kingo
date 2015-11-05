using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks
{
    internal sealed class ReadOnlyList<TValue> : IReadOnlyList<TValue>
    {
        private readonly IList<TValue> _list;

        internal ReadOnlyList(IList<TValue> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            _list = list;
        }

        public TValue this[int index]
        {
            get { return _list[index]; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
