using System.Transactions;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// When implemented by a class, manages the cache(s) that are maintained for queries.
    /// </summary>
    /// <remarks>
    /// Notes to implementers: all instance members of the class implementing this interface must be made thread-safe,
    /// because the instance will be shared and accessed among requests.
    /// </remarks>
    public interface IQueryCacheController : IDisposable
    {        
        /// <summary>
        /// Retrieves the cached result for <paramref name="message"/> from the application cache. If the
        /// entry did not yet exist, <paramref name="query" /> is executed to obtain the result and cache it
        /// for future requests before it is returned.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
        /// <param name="message">
        /// Message containing the parameters of the <paramref name="query"/> and all other execution options.
        /// </param>        
        /// <param name="query">The query to execute when a new cache item is to be added.</param>
        /// <returns>The (cached) result of <paramref name="query"/>.</returns>
        /// <exception cref="ObjectDisposedException">
        /// The current instance has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="QueryRequestMessage{TMessageIn}.Parameters" /> of <paramref name="message"/> has not been set to an instance of an object.
        /// </exception>
        TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>
            where TMessageOut : class, IMessage<TMessageOut>;

        /// <summary>
        /// Retrieves the cached result for <paramref name="message"/> from the session cache. If the
        /// entry did not yet exist, <paramref name="query" /> is executed to obtain the result and cache it
        /// for future requests before it is returned.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
        /// <param name="message">
        /// Message containing the parameters of the <paramref name="query"/> and all other execution options.
        /// </param>        
        /// <param name="query">The query to execute when a new cache item is to be added.</param>
        /// <returns>The (cached) result of <paramref name="query"/>.</returns>
        /// <exception cref="ObjectDisposedException">
        /// The current instance has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <see cref="QueryRequestMessage{TMessageIn}.Parameters" /> of <paramref name="message"/> has not been set to an instance of an object.
        /// </exception>
        TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
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
