namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents the argument of the event that is raised when a cache expired.
    /// </summary>
    public class CacheExpiredEventArgs : EventArgs
    {
        /// <summary>
        /// The cache that expired.
        /// </summary>
        public readonly object Cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheExpiredEventArgs" /> class.
        /// </summary>
        /// <param name="cache">The cache that expired.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cache"/> is <c>null</c>.
        /// </exception>
        public CacheExpiredEventArgs(object cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }
            Cache = cache;
        }
    }
}
