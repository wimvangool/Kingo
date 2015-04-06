using System.Resources;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// This attribute can be applied to messages to specify <see cref="IQuery{T, S}" /> request-messages to specify
    /// that it's results are to be cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class QueryCacheOptionsAttribute : Attribute, IQueryCacheStrategy
    {
        private readonly Lazy<QueryCacheStrategy> _queryCacheStrategy;
        private readonly QueryCacheKind _kind;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheOptionsAttribute" /> class.
        /// </summary>
        /// <param name="kind">Specifies which cache should be used to store the results in.</param>
        public QueryCacheOptionsAttribute(QueryCacheKind kind)
        {
            _queryCacheStrategy = new Lazy<QueryCacheStrategy>(CreateQueryCacheStrategy, true);
            _kind = kind;
        }

        /// <summary>
        /// Specifies which cache should be used to store the results in.
        /// </summary>
        public QueryCacheKind Kind
        {
            get { return _kind; }
        }

        /// <summary>
        /// Gets or sets the absolute expiration of the cached value, where <c>null</c> indicates a not set value.
        /// </summary>        
        public string AbsoluteExpiration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sliding expiration of the cached value, where <c>null</c> indicates a not set value.
        /// </summary>       
        public string SlidingExpiration
        {
            get;
            set;
        }

        private QueryCacheStrategy CreateQueryCacheStrategy()
        {
            return new QueryCacheStrategy(Kind, ParseTimeout(AbsoluteExpiration), ParseTimeout(SlidingExpiration));
        }

        TMessageOut IQueryCacheStrategy.GetOrAddToCache<TMessageOut>(IQuery<TMessageOut> query, QueryCacheModule module)
        {
            return _queryCacheStrategy.Value.GetOrAddToCache(query, module);
        }        

        private static TimeSpan? ParseTimeout(string timeout)
        {
            if (timeout == null)
            {
                return null;
            }
            try
            {
                return TimeSpan.Parse(timeout);
            }
            catch (FormatException exception)
            {
                throw NewInvalidTimeoutSpecifiedException(timeout, exception);
            }
        }        

        private static Exception NewInvalidTimeoutSpecifiedException(string timeout, Exception innerException)
        {
            var messageFormat = ExceptionMessages.QueryCacheOptionsAttribute_InvalidTimeoutSpecified;
            var message = string.Format(messageFormat, timeout);
            return new InvalidOperationException(message, innerException);
        }
    }
}
