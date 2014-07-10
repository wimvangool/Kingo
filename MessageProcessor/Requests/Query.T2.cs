using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a query that contains a <see cref="IMessage{T}">message</see> that serves
    /// as it's execution-parameter.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public abstract class Query<TMessage, TResult> : QueryBase<TResult>
        where TMessage : class, IMessage<TMessage>
    {
        private readonly TMessage _message;
        private readonly MessageStateTracker _messageStateTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{T1, T2}" /> class.
        /// </summary>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Query(TMessage message)
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

            _messageStateTracker.NotifyExecutionStarted(e.ExecutionId);
        }

        /// <inheritdoc />
        protected override void OnExecutionCanceled(ExecutionCanceledEventArgs e)
        {
            _messageStateTracker.NotifyExecutionEndedPrematurely(e.ExecutionId);

            base.OnExecutionCanceled(e);
        }

        /// <inheritdoc />
        protected override void OnExecutionFailed(ExecutionFailedEventArgs e)
        {
            _messageStateTracker.NotifyExecutionEndedPrematurely(e.ExecutionId);

            base.OnExecutionFailed(e);
        }

        #endregion

        #region [====== Execution ======]

        /// <inheritdoc />
        public override TResult Execute(QueryCache cache)
        {
            var executionId = Guid.NewGuid();
            var message = Message.Copy(true);

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId, message));
            TResult result;

            try
            {
                result = ExecuteQuery(message, cache, null);
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(executionId, message, exception));
                throw;
            }
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, message, result));

            return result;
        }

        /// <inheritdoc />
        public override async Task<TResult> ExecuteAsync(QueryCache cache, CancellationToken? token)
        {            
            var executionId = Guid.NewGuid();
            var message = Message.Copy(true);

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId, message));
            TResult result;

            if (TryGetFromCache(cache, message, out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, message, result));
                
                return result;
            }
            return await Start(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    result = ExecuteQuery(message, cache, token);

                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException exception)
                {
                    RequestContext.InvokeAsync(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(executionId, message, exception)));
                    throw;
                }
                catch (Exception exception)
                {
                    RequestContext.InvokeAsync(() => OnExecutionFailed(new ExecutionFailedEventArgs(executionId, message, exception)));
                    throw;
                }
                RequestContext.InvokeAsync(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, message, result)));

                return result;
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

        private TResult ExecuteQuery(TMessage message, QueryCache cache, CancellationToken? token)
        {
            return cache == null
                ? Execute(message, token)
                : cache.GetOrAdd(message, key => CreateCacheValue(key, Execute(key, token))).Access<TResult>();
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
        protected abstract TResult Execute(TMessage message, CancellationToken? token);                        

        #endregion
    }
}
