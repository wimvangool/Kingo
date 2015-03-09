using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// When implemented by a class, represents a strategy with which a query is executed and it's results are cached.
    /// </summary>
    public interface IQueryCacheStrategy
    {
        /// <summary>
        /// Attempts to retrieve a stored result of <paramref name="query"/> from the cache; if not present,
        /// <paramref name="query"/> is executed and it's result stored into the cache if the query's
        /// <see cref="QueryExecutionOptions" /> allow it.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="module">The module this strategy belongs to.</param>
        /// <returns>The result of <paramref name="query"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> or <paramref name="module"/> is <c>null</c>.
        /// </exception>
        TMessageOut GetOrAddToCache<TMessageOut>(IQuery<TMessageOut> query, QueryCacheModule module) where TMessageOut : class, IMessage<TMessageOut>;
    }
}
