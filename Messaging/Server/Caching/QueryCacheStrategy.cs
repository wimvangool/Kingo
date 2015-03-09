using System;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents a strategy with which a query is executed and it's results are cached.
    /// </summary>
    public sealed class QueryCacheStrategy : IQueryCacheStrategy
    {
        private readonly QueryCacheKind _kind;
        private readonly TimeSpan? _absoluteExpiration;
        private readonly TimeSpan? _slidingExpiration;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheStrategy" /> class.
        /// </summary>
        /// <param name="kind">Specifies which type of cache to use.</param>
        public QueryCacheStrategy(QueryCacheKind kind)
            : this(kind, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheStrategy" /> class.
        /// </summary>
        /// <param name="kind">Specifies which type of cache to use.</param>
        /// <param name="absoluteExpiration">Specifies a fixed lifetime for the cached value, or <c>null</c> for an indefinite lifetime.</param>
        /// <param name="slidingExpiration">Specifies a sliding lifetime for the cached value, or <c>null</c> for an indefinite lifetime.</param>
        public QueryCacheStrategy(QueryCacheKind kind, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
        {
            _kind = kind;
            _absoluteExpiration = absoluteExpiration;
            _slidingExpiration = slidingExpiration;
        }

        /// <inheritdoc />
        public TMessageOut GetOrAddToCache<TMessageOut>(IQuery<TMessageOut> query, QueryCacheModule module) where TMessageOut : class, IMessage<TMessageOut>
        {            
            if (_kind == QueryCacheKind.Application)
            {
                return module.GetOrAddToApplicationCache(query, _absoluteExpiration, _slidingExpiration);
            }
            if (_kind == QueryCacheKind.Session)
            {
                return module.GetOrAddToSessionCache(query, _absoluteExpiration, _slidingExpiration);
            }
            throw NewInvalidKindSpecifiedException(_kind);
        }

        private static Exception NewInvalidKindSpecifiedException(QueryCacheKind kind)
        {
            var messageFormat = ExceptionMessages.QueryCacheStrategy_InvalidKindSpecified;
            var message = string.Format(messageFormat, kind);
            return new InvalidOperationException(message);
        }
    }
}
