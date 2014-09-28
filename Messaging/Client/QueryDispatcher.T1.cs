using System.ComponentModel.Messaging.Server;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{    
    /// <summary>
    /// Represents a query that has no execution-parameter(s).
    /// </summary>
    /// <typeparam name="TResponse">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TResponse> : QueryDispatcherBase<TResponse> where TResponse : IMessage       
    {        
        #region [====== Execution ======]

        /// <inheritdoc />
        public override TResponse Execute(Guid requestId, ObjectCache cache)
        {                        
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));
            TResponse result;

            try
            {                
                result = ExecuteQuery(cache, null, null);                
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResponse>(requestId, result));

            return result;
        }

        /// <inheritdoc />
        public override Task<TResponse> ExecuteAsync(Guid requestId, ObjectCache cache, CancellationToken? token, IProgressReporter reporter)
        {                       
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));
            TResponse result;

            if (TryGetFromCache(cache, GetType(), out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResponse>(requestId, result));

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
                        scope.Post(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(requestId, exception)));
                        throw;
                    }
                    catch (Exception exception)
                    {
                        scope.Post(() => OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception)));
                        throw;
                    }                    
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResponse>(requestId, result)));

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
        protected virtual Task<TResponse> Start(Func<TResponse> query)
        {
            return Task<TResponse>.Factory.StartNew(query);
        }

        private TResponse ExecuteQuery(ObjectCache cache, CancellationToken? token, IProgressReporter reporter)
        {
            CacheItemPolicy policy;

            if (cache == null || !TryCreateCacheItemPolicy(out policy))
            {
                return Execute(token, reporter);
            }
            return cache.GetOrAdd(GetType(), () => Execute(token, reporter), policy);
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
        protected abstract TResponse Execute(CancellationToken? token, IProgressReporter reporter);

        #endregion        
    }
}
