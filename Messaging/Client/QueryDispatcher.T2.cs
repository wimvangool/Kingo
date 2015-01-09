using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a query that contains a <see cref="IRequestMessageViewModel">message</see> that serves
    /// as it's execution-parameter.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TMessageOut">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TMessageIn, TMessageOut> : QueryDispatcherBase<TMessageOut>
        where TMessageIn : class, IMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly TMessageIn _message;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcher{T1, T2}" /> class.
        /// </summary>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected QueryDispatcher(TMessageIn message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;            
        }

        /// <inheritdoc />
        public TMessageIn Message
        {
            get { return _message; }
        }        

        #region [====== Execution ======]

        /// <inheritdoc />
        public override TMessageOut Execute(Guid requestId)
        {                        
            var message = Message.Copy();

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));
            TMessageOut result;

            try
            {                
                result = ExecuteQuery(message, null);                
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, message, result));

            return result;
        }

        /// <inheritdoc />
        public override Task<TMessageOut> ExecuteAsync(Guid requestId, CancellationToken? token)
        {                        
            var message = Message.Copy();
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));
            TMessageOut result;
            
            if (TryGetFromCache(message, out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, message, result));

                return CreateCompletedTask(result);
            }
            return Start(() =>
            {
                using (var scope = new SynchronizationContextScope(context))
                {                   
                    try
                    {                        
                        result = ExecuteQuery(message, token);                        
                    }
                    catch (OperationCanceledException exception)
                    {
                        scope.Post(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(requestId, message, exception)));
                        throw;
                    }
                    catch (Exception exception)
                    {
                        scope.Post(() => OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception)));
                        throw;
                    }                    
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, message, result)));

                    return result;
                }
            });
        }

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task{T}" /> that is used to execute this query.
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

        private TMessageOut ExecuteQuery(TMessageIn message, CancellationToken? token)
        {
            CacheItemPolicy policy;

            if (Cache == null || !TryCreateCacheItemPolicy(out policy))
            {
                return (TMessageOut) Execute(message, token).Copy();
            }
            return (TMessageOut) Cache.GetOrAdd(message, () => Execute(message, token), policy).Copy();
        }        

        /// <summary>
        /// Executes the query.
        /// </summary>        
        /// <param name="message">The execution-parameter.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>                      
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>        
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TMessageOut Execute(TMessageIn message, CancellationToken? token);                        

        #endregion
    }
}
