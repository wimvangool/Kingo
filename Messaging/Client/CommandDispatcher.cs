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

            try
            {
                Execute(null);
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId));
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(Guid requestId, CancellationToken? token)
        {                                    
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));

            return Start(() =>
            {
                try
                {
                    Execute(token);
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
            });
        }

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task" /> that is used to execute this command.
        /// </summary>
        /// <param name="command">The action that will be invoked on the background thread.</param>
        /// <returns>The newly created task.</returns>
        /// <remarks>
        /// The default implementation uses the <see cref="TaskFactory.StartNew(Action)">StartNew</see>-method
        /// to start and return a new <see cref="Task" />. You may want to override this method to specify
        /// more options when creating this task.
        /// </remarks>
        protected virtual Task Start(Action command)
        {
            return Task.Factory.StartNew(command);
        }        

        /// <summary>
        /// Executes the command.
        /// </summary>   
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this command.
        /// </param>                   
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>        
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract void Execute(CancellationToken? token);

        #endregion
    }
}
