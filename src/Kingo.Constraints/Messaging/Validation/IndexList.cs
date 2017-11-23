using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging.Validation
{
    internal sealed class IndexList : IReadOnlyList<Tuple<Type, object>>
    {
        internal readonly Tuple<Type, object>[] Indices;

        internal IndexList(IEnumerable<Tuple<Type, object>> indices)
        {
            Indices = indices.Where(index => index != null).ToArray();
        }        

        internal IEnumerable<Type> Types()
        {
            return Indices.Select(index => index.Item1);
        }

        internal IEnumerable<object> Values()
        {
            return Indices.Select(index => index.Item2);
        }

        public override string ToString()
        {
            return "[" + string.Join(", ", Values().Select(index => index == null ? StringTemplate.NullValue : index.ToString())) + "]";
        }        

        #region [====== ReadOnlyList ======]

        public Tuple<Type, object> this[int index] =>
             Indices[index];

        public int Count =>
             Indices.Length;

        public IEnumerator<Tuple<Type, object>> GetEnumerator() =>
             Indices.OfType<Tuple<Type, object>>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Indices.GetEnumerator();

        #endregion
    }
}
