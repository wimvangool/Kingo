using System.Transactions;

namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// When implemented by a class, manages the cache(s) that are maintained for queries.
    /// </summary>
    /// <remarks>
    /// Notes to implementers: all instance members of the class implementing this interface must be made thread-safe,
    /// because the instance will be shared and accessed among requests.
    /// </remarks>
    public interface IQueryCacheProvider : IDisposable
    {        
        /// <summary>
        /// Retrieves the cached result for <paramref name="message"/> from the application cache. If the
        /// entry did not yet exist, <paramref name="query" /> is executed to obtain the result and cache it
        /// for future requests before it is returned, assuming the specified <paramref name="policy"/> allows it.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
        /// <param name="message">Message containing the parameters of the <paramref name="query"/>.</param>        
        /// <param name="query">The query to execute when a new cache item is to be added.</param>
        /// <param name="policy">The policy that is applied in caching the results of the specified <paramref name="query"/>.</param>
        /// <returns>The (cached) result of <paramref name="query"/>.</returns>
        /// <exception cref="ObjectDisposedException">
        /// The current instance has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>        
        TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryCachePolicy policy)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;

        /// <summary>
        /// Retrieves the cached result for <paramref name="message"/> from the session cache. If the
        /// entry did not yet exist, <paramref name="query" /> is executed to obtain the result and cache it
        /// for future requests before it is returned, assuming the specified <paramref name="policy"/> allows it.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
        /// <param name="message">Message containing the parameters of the <paramref name="query"/>.</param>        
        /// <param name="query">The query to execute when a new cache item is to be added.</param>
        /// <param name="policy">The policy that is applied in caching the results of the specified <paramref name="query"/>.</param>
        /// <returns>The (cached) result of <paramref name="query"/>.</returns>
        /// <exception cref="ObjectDisposedException">
        /// The current instance has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>        
        TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryCachePolicy policy)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;

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
