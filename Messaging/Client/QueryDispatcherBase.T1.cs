using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Serves as the base-class for the <see cref="QueryDispatcher{T}" /> and <see cref="QueryDispatcher{T, S}"/> classes,
    /// implementing the <see cref="IQueryDispatcher{T}" /> interface.
    /// </summary>
    public abstract class QueryDispatcherBase<TResponse> : RequestDispatcher, IQueryDispatcher<TResponse> where TResponse : IMessage
    {
        internal QueryDispatcherBase() { }        

        #region [====== ExecutionSucceeded ======]

        /// <summary>
        /// Occurs when an execution of this <see cref="ICommandDispatcher" /> has succeeded.
        /// </summary>
        public event EventHandler<ExecutionSucceededEventArgs<TResponse>> ExecutionSucceeded;

        internal override void Add(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded += new EventHandler<ExecutionSucceededEventArgs<TResponse>>(handler);
        }

        internal override void Remove(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded -= new EventHandler<ExecutionSucceededEventArgs<TResponse>>(handler);
        }

        /// <summary>
        /// Raises the <see cref="IRequestDispatcher.ExecutionSucceeded" /> and <see cref="RequestDispatcher.ExecutionCompleted"/> events.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionSucceeded(ExecutionSucceededEventArgs<TResponse> e)
        {
            ExecutionSucceeded.Raise(this, e);

            OnExecutionCompleted(e.ToExecutionCompletedEventArgs());
        }

        #endregion

        #region [====== Execution ======]

        /// <summary>
        /// Returns the cache that is used to store the results of this query.
        /// </summary>
        protected virtual ObjectCache Cache
        {
            get { return null; }
        }

        /// <inheritdoc />
        public abstract TResponse Execute(Guid requestId);            

        /// <inheritdoc />
        public Task<TResponse> ExecuteAsync(Guid requestId)
        {
            return ExecuteAsync(requestId, null);
        }                

        /// <inheritdoc />
        public abstract Task<TResponse> ExecuteAsync(Guid requestId, CancellationToken? token);

        /// <inheritdoc />
        public override IAsyncExecutionTask CreateAsyncExecutionTask()
        {
            return new QueryExecutionTask<TResponse>(this);
        }

        /// <summary>
        /// Attempts to create a <see cref="CacheItemPolicy" /> for a new result to store in cache.
        /// </summary>
        /// <param name="policy">
        /// When this method returns <c>true</c>, this paremeter will contain the newly created policy;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the policy was created; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// The default implementation creates a <see cref="CacheItemPolicy" /> with a sliding window expiration
        /// of 1 minute. Implementers can change this behavior by overridding this method to create another
        /// policy, or return <c>false</c> if they do not want the result to be cached at all.
        /// </remarks>
        protected virtual bool TryCreateCacheItemPolicy(out CacheItemPolicy policy)
        {
            policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a cached value from the cache, if <see cref="Cache" /> is not <c>null</c>.
        /// </summary>                
        /// <param name="key">The key of the cached value.</param>        
        /// <param name="result">
        /// If the method returns <c>true</c>, this parameter will contain the cached value after the method completes;
        /// will have the default value if this method returns <c>false</c>.
        /// </param>
        /// <returns><c>true</c> if the value was successfully retrieved from the cache; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The value retrieved from the cache could not be cast to <typeparamref name="TResponse"/>.
        /// </exception>
        protected bool TryGetFromCache(object key, out TResponse result)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (Cache == null)
            {
                result = default(TResponse);
                return false;
            }
            return Cache.TryGetValue(key, out result);
        }

        /// <summary>
        /// Creates and returns a task that is marked completed and returns the specified <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result is this query.</param>
        /// <returns>A new and completed <see cref="Task{T}"/>.</returns>
        internal static Task<TResponse> CreateCompletedTask(TResponse result)
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
        }

        #endregion
    }
}
