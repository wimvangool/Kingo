using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{    
    /// <summary>
    /// Represents a command that has no execution-parameter(s).
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public abstract class Query<TResult> : QueryBase<TResult>        
    {        
        #region [====== Execution ======]

        /// <inheritdoc />
        public override TResult Execute(IQueryCache cache)
        {
            var executionId = Guid.NewGuid();            

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));
            TResult result;

            try
            {
                result = ExecuteQuery(cache, null);
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception));
                throw;
            }
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result));

            return result;
        }

        /// <inheritdoc />
        public override async Task<TResult> ExecuteAsync(IDispatcher dispatcher, IQueryCache cache, CancellationToken? token)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            var executionId = Guid.NewGuid();           

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));
            TResult result;

            if (TryGetFromCache(cache, GetType(), out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result));

                return result;
            }
            return await Start(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    result = ExecuteQuery(cache, token);

                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException exception)
                {
                    dispatcher.Invoke(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(executionId, exception)));
                    throw;
                }
                catch (Exception exception)
                {
                    dispatcher.Invoke(() => OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception)));
                    throw;
                }
                dispatcher.Invoke(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result)));

                return result;
            });
        }

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task{T}" /> that is used to execute this command.
        /// </summary>
        /// <param name="query">The action that will be invoked on the background thread.</param>
        /// <returns>The newly created task.</returns>
        /// <remarks>
        /// The default implementation uses the <see cref="TaskFactory{T}.StartNew(Func{T})">StartNew</see>-method
        /// to start and return a new <see cref="Task{T}" />. You may want to override this method to specify
        /// more options when creating this task.
        /// </remarks>
        protected virtual Task<TResult> Start(Func<TResult> query)
        {
            return Task<TResult>.Factory.StartNew(query);
        }

        private TResult ExecuteQuery(IQueryCache cache, CancellationToken? token)
        {
            return cache == null
                ? Execute(token)
                : cache.GetOrAdd(GetType(), key => CreateCacheValue(key, Execute(token))).Access<TResult>();
        }

        /// <summary>
        /// Executes this query.
        /// </summary>        
        /// <returns>The result of this query.</returns>       
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TResult Execute(CancellationToken? token);

        #endregion
    }
}
