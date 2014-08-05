using System.ComponentModel.Messaging.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Serves as the base-class for the <see cref="QueryDispatcher{T}" /> and <see cref="QueryDispatcher{T, S}"/> classes,
    /// implementing the <see cref="IQueryDispatcher{T}" /> interface.
    /// </summary>
    public abstract class QueryDispatcherBase<TResult> : RequestDispatcher, IQueryDispatcher<TResult>
    {
        internal QueryDispatcherBase() { }

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
        public Task<TResult> ExecuteAsync(Guid executionId)
        {
            return ExecuteAsync(executionId, null, null, null);
        }        

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync(Guid executionId, QueryCache cache)
        {
            return ExecuteAsync(executionId, cache, null, null);
        }

        /// <inheritdoc />
        public abstract Task<TResult> ExecuteAsync(Guid executionId, QueryCache cache, CancellationToken? token, IProgressReporter reporter);

        /// <inheritdoc />
        public override IAsyncExecutionTask CreateAsyncExecutionTask()
        {
            return new QueryExecutionTask<TResult>(this, null);
        }

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
        /// Creates and returns a task that is marked completed and returns the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result is this query.</param>
        /// <returns>A new and completed <see cref="Task{T}"/>.</returns>
        internal static Task<TResult> CreateCompletedTask(TResult result)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
        }

        #endregion
    }
}
