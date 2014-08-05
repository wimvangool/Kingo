using System.ComponentModel.Messaging.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{    
    /// <summary>
    /// Represents a command that has no execution-parameter(s).
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TResult> : QueryDispatcherBase<TResult>        
    {        
        #region [====== Execution ======]

        /// <inheritdoc />
        public override TResult Execute(QueryCache cache)
        {            
            var executionId = Guid.NewGuid();            

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));
            TResult result;

            try
            {                
                result = ExecuteQuery(cache, null, null);                
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
        public override Task<TResult> ExecuteAsync(Guid executionId, QueryCache cache, CancellationToken? token, IProgressReporter reporter)
        {                       
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));
            TResult result;

            if (QueryCache.TryGetFromCache(cache, GetType(), out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result));

                return CreateCompletedTask(result);
            }
            return Start(() =>
            {
                using (var scope = new SynchronizationContextScope(context))
                {                   
                    try
                    {                        
                        result = ExecuteQuery(cache, token, reporter);                                                
                    }
                    catch (OperationCanceledException exception)
                    {
                        scope.Post(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(executionId, exception)));
                        throw;
                    }
                    catch (Exception exception)
                    {
                        scope.Post(() => OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception)));
                        throw;
                    }                    
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result)));

                    return result;
                }
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

        private TResult ExecuteQuery(QueryCache cache, CancellationToken? token, IProgressReporter reporter)
        {
            return cache == null
                ? ExecuteInTransactionScope(token, reporter)
                : cache.GetOrAdd(GetType(), key => CreateCacheValue(key, ExecuteInTransactionScope(token, reporter))).Access<TResult>();
        }

        private TResult ExecuteInTransactionScope(CancellationToken? token, IProgressReporter reporter)
        {
            TResult result;

            token.ThrowIfCancellationRequested();

            using (var scope = CreateTransactionScope())
            {
                result = Execute(token, reporter);

                scope.Complete();
            }
            return result;
        }

        /// <summary>
        /// Executes this query.
        /// </summary>        
        /// <param name="token">
        /// Token that can be used to cancel the task.
        /// </param>
        /// <param name="reporter">
        /// Reporter that can be used to report the progress.
        /// </param>
        /// <returns>The result of this query.</returns>       
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TResult Execute(CancellationToken? token, IProgressReporter reporter);

        #endregion        
    }
}
