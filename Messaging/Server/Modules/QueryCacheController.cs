using System.Transactions;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheController" /> interface.
    /// </summary>
    public abstract class QueryCacheController : IQueryCacheController
    {
        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether or not the current instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region [====== Caching ======]

        TMessageOut IQueryCacheController.GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            return GetOrAddToApplicationCache(message, absoluteExpiration, slidingExpiration, query);
        }

        TMessageOut IQueryCacheController.GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            return GetOrAddToSessionCache(message, absoluteExpiration, slidingExpiration, query);
        }

        void IQueryCacheController.InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (mustInvalidate == null)
            {
                throw new ArgumentNullException("mustInvalidate");
            }
            InvalidateIfRequired(mustInvalidate);
        }

        /// <summary>
        /// Retrieves the cached result for <paramref name="message"/> from the application cache. If the
        /// entry did not yet exist, <paramref name="query" /> is executed to obtain the result and cache it
        /// for future requests before it is returned.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
        /// <param name="message">Message containing the parameters of <paramref name="query"/>.</param>
        /// <param name="absoluteExpiration">Optional timeout value that causes the cached result to expire after a fixed amount of time.</param>
        /// <param name="slidingExpiration">Optional timeout value that causes the cached result to expire after a certain amount of unused time.</param>
        /// <param name="query">The query to execute when a new cache item is to be added.</param>
        /// <returns>The (cached) result of <paramref name="query"/>.</returns>   
        protected abstract TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>;

        /// <summary>
        /// Retrieves the cached result for <paramref name="message"/> from the session cache. If the
        /// entry did not yet exist, <paramref name="query" /> is executed to obtain the result and cache it
        /// for future requests before it is returned.
        /// </summary>
        /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
        /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
        /// <param name="message">Message containing the parameters of <paramref name="query"/>.</param>
        /// <param name="absoluteExpiration">Optional timeout value that causes the cached result to expire after a fixed amount of time.</param>
        /// <param name="slidingExpiration">Optional timeout value that causes the cached result to expire after a certain amount of unused time.</param>
        /// <param name="query">The query to execute when a new cache item is to be added.</param>
        /// <returns>The (cached) result of <paramref name="query"/>.</returns>        
        protected abstract TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>;

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
        protected abstract void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class;

        #endregion
    }
}
