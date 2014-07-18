using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Messages
{
    /// <summary>
    /// Serves as the base-class for the <see cref="QueryDispatcher{T}" /> and <see cref="QueryDispatcher{T, S}"/> classes,
    /// implementing the <see cref="IQueryDispatcher{T}" /> interface.
    /// </summary>
    public abstract class QueryBaseDispatcher<TResult> : RequestDispatcher, IQueryDispatcher<TResult>
    {
        internal QueryBaseDispatcher() { }

        #region [====== ExecutionSucceeded ======]

        /// <summary>
        /// Occurs when an execution of this <see cref="ICommandDispatcher" /> has succeeded.
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
        /// Raises the <see cref="IRequestDispatcher.ExecutionSucceeded" /> and <see cref="RequestDispatcher.ExecutionCompleted"/> events.
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
        public abstract TResult Execute(QueryCache cache);

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync()
        {
            return ExecuteAsync(null, null);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync(CancellationToken? token)
        {
            return ExecuteAsync(null, token);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync(QueryCache cache)
        {
            return ExecuteAsync(cache, null);
        }

        /// <inheritdoc />
        public abstract Task<TResult> ExecuteAsync(QueryCache cache, CancellationToken? token);

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
            return new QueryCacheValue(result);
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
        protected static bool TryGetFromCache(QueryCache cache, object key, out TResult result)
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

        /// <summary>
        /// Creates and returns a task that is marked completed and returns the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result is this query.</param>
        /// <returns>A new and completed <see cref="Task{T}"/>.</returns>
        protected static Task<TResult> CreateCompletedTask(TResult result)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
        }

        #endregion
    }
}
