using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// This module is used to cache the results of <see cref="IQuery{TMessageIn, TMessageOut}">Queries</see>.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public class QueryCacheModule<TMessageIn, TMessageOut> : QueryModule<TMessageIn, TMessageOut, QueryCacheOptionsAttribute>
        where TMessageIn : class, IMessage<TMessageIn>
    {
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly QueryExecutionOptions _options;
        private readonly IQueryCacheController _cacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheModule{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="query">The next query to invoke.</param>
        /// <param name="options">Specify how the query should be executed.</param>
        /// <param name="cacheManager">The manager that is used to manage the cahces that are used.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="cacheManager"/> is <c>null</c>.
        /// </exception>        
        public QueryCacheModule(IQuery<TMessageIn, TMessageOut> query, QueryExecutionOptions options, IQueryCacheController cacheManager)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (cacheManager == null)
            {
                throw new ArgumentNullException("cacheManager");
            }
            _query = query;
            _options = options;
            _cacheManager = cacheManager;
        }

        /// <inheritdoc />
        protected override IQuery<TMessageIn, TMessageOut> Query
        {
            get { return _query; }
        }

        /// <summary>
        /// Specify how the query should be executed.
        /// </summary>
        protected virtual QueryExecutionOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// The manager that is used to manage the cahces that are used.
        /// </summary>
        protected virtual IQueryCacheController CacheManager
        {
            get { return _cacheManager; }
        }

        /// <summary>
        /// Executes the query and caches its result following the specified configuration.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="attributes">All attributes of type <see cref="QueryCacheOptionsAttribute"/> declared on <paramref name="message"/>.</param>
        /// <returns>The result of the query.</returns>
        protected override TMessageOut Execute(TMessageIn message, IEnumerable<QueryCacheOptionsAttribute> attributes)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var attribute = attributes.SingleOrDefault();
            if (attribute == null)
            {
                return Query.Execute(message);    
            }
            return attribute.GetOrAddToCache(message, Query, Options, CacheManager);
        }
    }
}
