using System.ComponentModel.Messaging.Server;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a query that contains a <see cref="IMessage">message</see> that serves
    /// as it's execution-parameter.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TMessage, TResult> : QueryDispatcherBase<TResult> where TMessage : class, IMessage
    {
        private readonly TMessage _message;
        private readonly MessageStateTracker _messageStateTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDispatcher{T1, T2}" /> class.
        /// </summary>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected QueryDispatcher(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;
            _messageStateTracker = new MessageStateTracker(message);
        }

        /// <inheritdoc />
        public TMessage Message
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
        public override TResult Execute(Guid requestId, ObjectCache cache)
        {                        
            var message = (TMessage) Message.Copy(true);

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));
            TResult result;

            try
            {                
                result = ExecuteQuery(message, cache, null, null);                
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(requestId, message, result));

            return result;
        }

        /// <inheritdoc />
        public override Task<TResult> ExecuteAsync(Guid requestId, ObjectCache cache, CancellationToken? token, IProgressReporter reporter)
        {                        
            var message = (TMessage) Message.Copy(true);
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));
            TResult result;
            
            if (TryGetFromCache(cache, message, out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(requestId, message, result));

                return CreateCompletedTask(result);
            }
            return Start(() =>
            {
                using (var scope = new SynchronizationContextScope(context))
                {                   
                    try
                    {                        
                        result = ExecuteQuery(message, cache, token, reporter);                        
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
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(requestId, message, result)));

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
        protected virtual Task<TResult> Start(Func<TResult> query)
        {
            return Task<TResult>.Factory.StartNew(query);
        }

        private TResult ExecuteQuery(TMessage message, ObjectCache cache, CancellationToken? token, IProgressReporter reporter)
        {
            CacheItemPolicy policy;

            if (cache == null || !TryCreateCacheItemPolicy(out policy))
            {
                return ExecuteInTransactionScope(message, token, reporter);
            }
            return cache.GetOrAdd(message, () => ExecuteInTransactionScope(message, token, reporter), policy);
        }

        private TResult ExecuteInTransactionScope(TMessage message, CancellationToken? token, IProgressReporter reporter)
        {
            TResult result;

            token.ThrowIfCancellationRequested();

            using (var scope = CreateTransactionScope())
            {
                result = Execute(message, token, reporter);

                scope.Complete();
            }
            return result;
        }

        /// <summary>
        /// Executes the query.
        /// </summary>        
        /// <param name="message">The execution-parameter.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>              
        /// <param name="reporter">
        /// Reporter that can be used to report the progress.
        /// </param>  
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>        
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TResult Execute(TMessage message, CancellationToken? token, IProgressReporter reporter);                        

        #endregion
    }
}
