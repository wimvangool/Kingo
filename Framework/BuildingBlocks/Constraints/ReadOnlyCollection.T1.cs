using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ReadOnlyCollection<TValue> : IReadOnlyCollection<TValue>
    {
        private readonly ICollection<TValue> _collection;

        internal ReadOnlyCollection(ICollection<TValue> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            _collection = collection;
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
