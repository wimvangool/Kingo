using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{    
    /// <summary>
    /// Represents a query that has no execution-parameter(s).
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TMessageOut> : QueryDispatcherBase<TMessageOut> where TMessageOut : class, IMessage<TMessageOut>       
    {        
        #region [====== Execution ======]

        /// <inheritdoc />
        public override TMessageOut Execute(Guid requestId)
        {                        
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));
            TMessageOut result;

            try
            {                
                result = ExecuteQuery(null);                
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, result));

            return result;
        }

        /// <inheritdoc />
        public override Task<TMessageOut> ExecuteAsync(Guid requestId, CancellationToken? token)
        {                       
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));
            TMessageOut result;

            if (TryGetFromCache(GetType(), out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, result));

                return CreateCompletedTask(result);
            }
            return Start(() =>
            {
                using (var scope = new SynchronizationContextScope(context))
                {                   
                    try
                    {                        
                        result = ExecuteQuery(token);                                                
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
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, result)));

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
        protected virtual Task<TMessageOut> Start(Func<TMessageOut> query)
        {
            return Task<TMessageOut>.Factory.StartNew(query);
        }

        private TMessageOut ExecuteQuery(CancellationToken? token)
        {
            CacheItemPolicy policy;

            if (Cache == null || !TryCreateCacheItemPolicy(out policy))
            {
                return (TMessageOut) Execute(token).Copy();
            }
            return (TMessageOut) Cache.GetOrAdd(GetType(), () => Execute(token), policy).Copy();
        }        

        /// <summary>
        /// Executes this query.
        /// </summary>        
        /// <param name="token">
        /// Token that can be used to cancel the task.
        /// </param>        
        /// <returns>The result of this query.</returns>       
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TMessageOut Execute(CancellationToken? token);

        #endregion        
    }
}
