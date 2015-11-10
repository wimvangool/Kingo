using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class IndexList : IReadOnlyList<object>
    {
        internal readonly object[] Indices;

        internal IndexList(IEnumerable<object> indices)
        {
            if (indices == null)
            {
                throw new ArgumentNullException("indices");
            }
            var indexList = indices.ToArray();
            if (indexList.Length == 0)
            {
                throw NewEmptyIndexListException("indexList");
            }
            Indices = indexList;
        }        

        internal IEnumerable<Type> IndexTypes()
        {
            return Indices.Select(index => index == null ? typeof(object) : index.GetType());
        }  

        public override string ToString()
        {
            return "[" + string.Join(", ", Indices.Select(index => index == null ? StringTemplate.NullValue : index.ToString())) + "]";
        }

        private static Exception NewEmptyIndexListException(string paramName)
        {
            return new ArgumentException(ExceptionMessages.IndexList_EmptyList, paramName);
        }

        #region [====== ReadOnlyList ======]

        public object this[int index]
        {
            get { return Indices[index]; }
        }

        public int Count
        {
            get { return Indices.Length; }
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var index in Indices)
            {
                yield return index;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Indices.GetEnumerator();
        }

        #endregion
    }
}
