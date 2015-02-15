using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{    
    /// <summary>
    /// Represents a command that has no execution-parameter(s).
    /// </summary>
    public abstract class CommandDispatcher : CommandDispatcherBase
    {                       
        #region [====== Execution ======]

        /// <inheritdoc />
        public override void Execute(Guid requestId)
        {                        
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));            

            Processor.Handle(new RequestMessage(null), msg =>
            {
                try
                {
                    Execute();
                }
                catch (Exception exception)
                {
                    OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception));
                    throw;
                }
                OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId));    
            });            
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(Guid requestId, CancellationToken? token)
        {                                    
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));

            return Processor.HandleAsync(new RequestMessage(null), msg =>
            {
                try
                {
                    Execute();
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
                Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId)));
            }, null, token);
        }             

        /// <summary>
        /// Executes the command.
        /// </summary>                   
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract void Execute();

        #endregion
    }
}
