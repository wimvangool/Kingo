using System.Collections.Generic;
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
            }, null, options);            
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
            }, null, options, token);            
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

        #region [====== RequestMessageDispatcher ======]

        IEnumerable<TAttribute> IRequestMessageDispatcher.SelectAttributesOfType<TAttribute>()
        {
            return SelectAttributesOfType<TAttribute>();
        }

        /// <summary>
        /// Returns a collection of <see cref="Attribute">MessageAttributes</see> that are
        /// declared on this dispatcher and are an instance of <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attributes to select.</typeparam>        
        /// <returns>A collection of <see cref="Attribute">MessageAttributes</see>.</returns>
        protected virtual IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>() where TAttribute : Attribute
        {
            return Message.AttributesOfType<TAttribute>(GetType());
        }

        #endregion   
    }
}
