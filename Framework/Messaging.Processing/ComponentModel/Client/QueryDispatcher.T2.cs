using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Client
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

            return Processor.Execute(message, msg =>
            {
                TMessageOut result;

                try
                {
                    result = Execute(message);
                }
                catch (Exception exception)
                {
                    OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception));
                    throw;
                }
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, message, result));

                return result;
            });            
        }

        /// <inheritdoc />
        public override Task<TMessageOut> ExecuteAsync(Guid requestId, CancellationToken token)
        {                        
            var message = Message.Copy();            

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));            
                        
            return Processor.ExecuteAsync(message, msg =>
            {
                TMessageOut result;

                try
                {
                    result = Execute(message);
                }
                catch (OperationCanceledException exception)
                {
                    Post(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(requestId, message, exception)));
                    throw;
                }
                catch (Exception exception)
                {
                    Post(() => OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception)));
                    throw;
                }
                Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TMessageOut>(requestId, message, result)));

                return result;
            }, token);
        }              

        /// <summary>
        /// Executes the query.
        /// </summary>        
        /// <param name="message">Message containing the parameters of the query.</param>              
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TMessageOut Execute(TMessageIn message);                        

        #endregion
    }
}
