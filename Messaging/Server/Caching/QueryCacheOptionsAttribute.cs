using System.ComponentModel.Resources;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// This attribute can be applied to messages to specify <see cref="IQuery{T, S}" /> request-messages to specify
    /// that it's results are to be cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class QueryCacheOptionsAttribute : MessageAttribute
    {
        private readonly QueryCacheKind _kind;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheOptionsAttribute" /> class.
        /// </summary>
        /// <param name="kind">Specifies which cache should be used to store the results in.</param>
        public QueryCacheOptionsAttribute(QueryCacheKind kind)
        {
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
    
        internal TMessageOut GetOrAddToCache<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryExecutionOptions options, IQueryCacheProvider cacheManager)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>
        {           
            var absoluteExpiration = ParseTimeout(AbsoluteExpiration);
            var slidingExpiration = ParseTimeout(SlidingExpiration);
            var policy = new QueryCachePolicy(options, absoluteExpiration, slidingExpiration);
            
            if (Kind == QueryCacheKind.Application)
            {
                return cacheManager.GetOrAddToApplicationCache(message, query, policy);
            }
            if (Kind == QueryCacheKind.Session)
            {
                return cacheManager.GetOrAddToSessionCache(message, query, policy);
            }
            throw NewInvalidKindSpecifiedException(Kind);
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

        private static Exception NewInvalidKindSpecifiedException(QueryCacheKind kind)
        {
            var messageFormat = ExceptionMessages.QueryCacheOptions_InvalidKindSpecified;
            var message = string.Format(messageFormat, kind);
            return new InvalidOperationException(message);
        }

        private static Exception NewInvalidTimeoutSpecifiedException(string timeout, Exception innerException)
        {
            var messageFormat = ExceptionMessages.QueryCacheOptions_InvalidTimeoutSpecified;
            var message = string.Format(messageFormat, timeout);
            return new InvalidOperationException(message, innerException);
        }
    }
}
