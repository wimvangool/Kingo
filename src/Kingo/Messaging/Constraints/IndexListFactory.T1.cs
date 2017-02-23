using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// Represents a list of delegates that are used to obtain the type and the argument-values for indexer-invocations.
    /// </summary>
    /// <typeparam name="T">Type of the instance that will serve as the input for the delegates.</typeparam>
    public sealed class IndexListFactory<T> : IReadOnlyList<Delegate>
    {        
        private readonly List<Delegate> _indexFactoryList;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexList" /> class.
        /// </summary>
        public IndexListFactory()
        {            
            _indexFactoryList = new List<Delegate>(4);
        }

        /// <inheritdoc />
        public int Count
        {
            get { return _indexFactoryList.Count; }
        }

        Delegate IReadOnlyList<Delegate>.this[int index]
        {
            get { return _indexFactoryList[index]; }
        }

        /// <summary>
        /// Adds a new index-delegate to this list.
        /// </summary>
        /// <typeparam name="TValue">Type of the return-value of the delegate.</typeparam>
        /// <param name="indexFactory">The delegate to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indexFactory"/> is <c>null</c>.
        /// </exception>
        public void Add<TValue>(Func<T, TValue> indexFactory)
        {
            if (indexFactory == null)
            {
                throw new ArgumentNullException(nameof(indexFactory));
            }
            _indexFactoryList.Add(indexFactory);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "[" + string.Join(", ", _indexFactoryList.Select(indexFactory => indexFactory.Method.ReturnType)) + "]";
        } 

        internal IndexList Materialize(T instance)
        {
            var indexList =
                from indexFactory in _indexFactoryList
                let type = indexFactory.Method.ReturnType
                let value = indexFactory.DynamicInvoke(instance)
                select new Tuple<Type, object>(type, value);

            return new IndexList(indexList);
        }       

        /// <inheritdoc />
        public IEnumerator<Delegate> GetEnumerator()
        {
            return _indexFactoryList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
