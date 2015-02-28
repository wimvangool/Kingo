using System.Transactions;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// When implemented by a class, manages the cache(s) that are maintained for queries.
    /// </summary>    
    public interface IQueryCacheModule : IQueryModule
    {                
        /// <summary>
        /// Detects if any results of the specified <typeparamref name="TMessageIn"/> were changed,
        /// causing the related cache entries to be invalidated.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the query to check.</typeparam>
        /// <param name="mustInvalidate">
        /// Indicates of a specific result of <typeparamref name="TMessageIn"/> has changed. If this
        /// function returns <c>true</c>, the related cached result is invalidated as soon as any
        /// existing <see cref="Transaction" /> has committed; otherwise, nothing happens.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The current instance has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mustInvalidate"/> is <c>null</c>.
        /// </exception>
        void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class;        
    }
}
