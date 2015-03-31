namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents a factory with which new <see cref="QueryCacheManager">QueryCacheManagers</see> can be created
    /// for a specific cache instance.
    /// </summary>
    public interface IQueryCacheManagerFactory
    {
        /// <summary>
        /// The cache to create the manager for.
        /// </summary>
        object Cache
        {
            get;
        }

        /// <summary>
        /// Creates a new <see cref="QueryCacheManager" /> for the embedded <see cref="Cache"/>.
        /// </summary>
        /// <returns>A new <see cref="QueryCacheManager" /> for the embedded <see cref="Cache"/>.</returns>
        QueryCacheManager CreateCacheManager();
    }
}
