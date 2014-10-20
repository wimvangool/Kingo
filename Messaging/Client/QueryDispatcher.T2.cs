using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a query that contains a <see cref="IRequestMessage">message</see> that serves
    /// as it's execution-parameter.
    /// </summary>
    /// <typeparam name="TRequest">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TResponse">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TRequest, TResponse> : QueryDispatcherBase<TResponse>
        where TRequest : class, IRequestMessage
        where TResponse : IMessage
    {
        private readonly TRequest _message;
        private readonly RequestMessageStateTracker _messageStateTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcher{T1, T2}" /> class.
        /// </summary>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected QueryDispatcher(TRequest message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;
            _messageStateTracker = new RequestMessageStateTracker(message);
        }

        /// <inheritdoc />
        public TRequest Message
        {
            get { return _message; }
        }

        #region [====== Events ======]

        /// <inheritdoc />
        protected override void OnExecutionStarted(ExecutionStartedEventArgs e)
        {
            base.OnExecutionStarted(e);

            _messageStateTracker.NotifyExecutionStarted(e.RequestId);
        }

        /// <inheritdoc />
        protected override void OnExecutionCanceled(ExecutionCanceledEventArgs e)
        {
            _messageStateTracker.NotifyExecutionEndedPrematurely(e.RequestId);

            base.OnExecutionCanceled(e);
        }

        /// <inheritdoc />
        protected override void OnExecutionFailed(ExecutionFailedEventArgs e)
        {
            _messageStateTracker.NotifyExecutionEndedPrematurely(e.RequestId);

            base.OnExecutionFailed(e);
        }

        #endregion

        #region [====== Execution ======]

        /// <inheritdoc />
        public override TResponse Execute(Guid requestId)
        {                        
            var message = (TRequest) Message.Copy(true);

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));
            TResponse result;

            try
            {                
                result = ExecuteQuery(message, null);                
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResponse>(requestId, message, result));

            return result;
        }

        /// <inheritdoc />
        public override Task<TResponse> ExecuteAsync(Guid requestId, CancellationToken? token)
        {                        
            var message = (TRequest) Message.Copy(true);
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));
            TResponse result;
            
            if (TryGetFromCache(message, out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResponse>(requestId, message, result));

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
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResponse>(requestId, message, result)));

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
        protected virtual Task<TResponse> Start(Func<TResponse> query)
        {
            return Task<TResponse>.Factory.StartNew(query);
        }

        private TResponse ExecuteQuery(TRequest message, CancellationToken? token)
        {
            CacheItemPolicy policy;

            if (Cache == null || !TryCreateCacheItemPolicy(out policy))
            {
                return (TResponse) Execute(message, token).Copy();
            }
            return (TResponse) Cache.GetOrAdd(message, () => Execute(message, token), policy).Copy();
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
        protected abstract TResponse Execute(TRequest message, CancellationToken? token);                        

        #endregion
    }
}
