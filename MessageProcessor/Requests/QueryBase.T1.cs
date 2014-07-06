using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Serves as the base-class for the <see cref="Query{T}" /> and <see cref="Query{T, S}"/> classes,
    /// implementing the <see cref="IQuery{T}" /> interface.
    /// </summary>
    public abstract class QueryBase<TResult> : Request, IQuery<TResult>
    {
        internal QueryBase() { }

        #region [====== ExecutionSucceeded ======]

        /// <summary>
        /// Occurs when an execution of this <see cref="ICommand" /> has succeeded.
        /// </summary>
        public event EventHandler<ExecutionSucceededEventArgs<TResult>> ExecutionSucceeded;

        internal override void Add(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded += new EventHandler<ExecutionSucceededEventArgs<TResult>>(handler);
        }

        internal override void Remove(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded -= new EventHandler<ExecutionSucceededEventArgs<TResult>>(handler);
        }

        /// <summary>
        /// Raises the <see cref="IRequest.ExecutionSucceeded" /> and <see cref="Request.ExecutionCompleted"/> events.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionSucceeded(ExecutionSucceededEventArgs<TResult> e)
        {
            ExecutionSucceeded.Raise(this, e);

            OnExecutionCompleted(e.ToExecutionCompletedEventArgs());
        }

        #endregion

        #region [====== Execution ======]

        /// <inheritdoc />
        public TResult Execute()
        {
            return Execute(null);
        }

        /// <inheritdoc />
        public abstract TResult Execute(IQueryCache cache);

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync(IDispatcher dispatcher)
        {
            return await ExecuteAsync(dispatcher, null, null);
        }

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync(IDispatcher dispatcher, CancellationToken? token)
        {
            return await ExecuteAsync(dispatcher, null, token);
        }

        /// <inheritdoc />
        public async Task<TResult> ExecuteAsync(IDispatcher dispatcher, IQueryCache cache)
        {
            return await ExecuteAsync(dispatcher, cache, null);
        }

        /// <inheritdoc />
        public abstract Task<TResult> ExecuteAsync(IDispatcher dispatcher, IQueryCache cache, CancellationToken? token);

        /// <summary>
        /// Wraps the specified result into a new <see cref="QueryCacheValue" />.
        /// </summary>
        /// <param name="key">The key that will be used to store this value.</param>
        /// <param name="result">The result returned by this query.</param>
        /// <returns>A new <see cref="QueryCacheValue" />.</returns>
        /// <remarks>
        /// By default, this method returns a value with an infinite lifetime. You may want to override this method
        /// to specify another lifetime for the cached value.
        /// </remarks>
        protected virtual QueryCacheValue CreateCacheValue(object key, TResult result)
        {
            return QueryCacheValue.WithInfiniteLifetime(result);
        }

        /// <summary>
        /// Attempts to read a result from the cache.
        /// </summary>
        /// <param name="cache">The cache to read the result from (optional).</param>
        /// <param name="key">The key used to read the result.</param>
        /// <param name="result">
        /// If <paramref name="cache"/> is not <c>null</c> and the cache contains the
        /// value stored by the specified <paramref name="key"/>, this parameter will contain
        /// the stored result.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was succesfully read from the cache; otherwise <c>false</c>.
        /// </returns>
        protected static bool TryGetFromCache(IQueryCache cache, object key, out TResult result)
        {
            QueryCacheValue value;

            if (cache != null && cache.TryGetValue(key, out value))
            {
                result = value.Access<TResult>();
                return true;
            }
            result = default(TResult);
            return false;
        }

        #endregion
    }
}
