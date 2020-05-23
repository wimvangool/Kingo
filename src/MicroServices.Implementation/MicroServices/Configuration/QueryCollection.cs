using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of query-types.
    /// </summary>
    public sealed class QueryCollection : MicroProcessorComponentCollection
    {
        private readonly Dictionary<Type, QueryType> _queries;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCollection" /> class.
        /// </summary>
        public QueryCollection()
        {
            _queries = new Dictionary<Type, QueryType>();
        }

        /// <inheritdoc />
        public override IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            _queries.Values.GetEnumerator();

        #region [====== Add ======]

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (QueryType.IsQuery(component, out var query))
            {
                _queries[query.Type] = query;
                return true;
            }
            return false;
        }

        #endregion
    }
}
