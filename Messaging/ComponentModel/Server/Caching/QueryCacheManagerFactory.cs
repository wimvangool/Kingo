namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheManagerFactory" /> interface for a
    /// <typeparamref name="TCache">specific type of</typeparamref> cache.
    /// </summary>
    /// <typeparam name="TCache">Type of the cache the managers are created for.</typeparam>
    public abstract class QueryCacheManagerFactory<TCache> : IQueryCacheManagerFactory where TCache : class
    {
        private readonly TCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCacheManagerFactory{TCache}" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> is <c>null</c>.
        /// </exception>
        protected QueryCacheManagerFactory(TCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            _cache = cache;
        }

        object IQueryCacheManagerFactory.Cache
        {
            get { return _cache; }
        }

        /// <summary>
        /// The cache to create the manager for.
        /// </summary>
        protected TCache Cache
        {
            get { return _cache; }
        }

        /// <inheritdoc />
        public abstract QueryCacheManager CreateCacheManager();
    }
}
