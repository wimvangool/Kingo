using System.ComponentModel.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{    
    /// <summary>
    /// Represents a query that has no execution-parameter(s).
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TMessageOut> : QueryDispatcherBase<TMessageOut>, IRequestMessageDispatcher where TMessageOut : class, IMessage<TMessageOut>
    {
        #region [====== RequestMessageDispatcher ======]

        string IRequestMessageDispatcher.MessageTypeId
        {
            get { return MessageTypeId; }
        }

        /// <summary>
        /// Returns the identifier of the message that is dispatched by this dispatcher.
        /// </summary>
        protected abstract string MessageTypeId
        {
            get;
        }

        #endregion

        #region [====== Execution ======]

        /// <inheritdoc />
        public override TMessageOut Execute(Guid requestId, QueryExecutionOptions options = QueryExecutionOptions.Default)
        {                        
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));

            return Processor.Execute(new RequestMessage(this), msg =>
            {
                TMessageOut result;

                try
                {
                    result = Execute();
                }
                catch (Exception exception)
                {
                    OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception));
                    throw;
                }
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, result));

                return result;
            }, options);            
        }

        /// <inheritdoc />
        public override Task<TMessageOut> ExecuteAsync(Guid requestId, QueryExecutionOptions options = QueryExecutionOptions.Default, CancellationToken? token = null)
        {                                   
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));

            return Processor.ExecuteAsync(new RequestMessage(this), msg =>
            {
                TMessageOut result;

                try
                {
                    result = Execute();
                }
                catch (OperationCanceledException exception)
                {
                    Post(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(requestId, exception)));
                    throw;
                }
                catch (Exception exception)
                {
                    Post(() => OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception)));
                    throw;
                }
                Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, result)));

                return result;
            }, options, token);            
        }             

        /// <summary>
        /// Executes this query.
        /// </summary>                    
        /// <returns>The result of this query.</returns>       
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TMessageOut Execute();        

        #endregion                
    }
}
