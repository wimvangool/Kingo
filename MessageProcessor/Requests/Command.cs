using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{    
    /// <summary>
    /// Represents a command that has no execution-parameter(s).
    /// </summary>
    public abstract class Command : CommandBase
    {                       
        #region [====== Execution ======]

        /// <inheritdoc />
        public override void Execute()
        {
            var executionId = Guid.NewGuid();            

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));

            try
            {
                Execute(null);
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception));
                throw;
            }
            OnExecutionSucceeded(new ExecutionSucceededEventArgs(executionId));
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(CancellationToken? token)
        {            
            var executionId = Guid.NewGuid();
            var requestContext = RequestContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));

            await Start(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    Execute(token);                    
                }
                catch (OperationCanceledException exception)
                {
                    requestContext.InvokeAsync(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(executionId, exception)));
                    throw;
                }
                catch (Exception exception)
                {
                    requestContext.InvokeAsync(() => OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception)));
                    throw;
                }
                requestContext.InvokeAsync(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs(executionId)));
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
        /// <exception cref="RequestExecutionException">
        /// The command failed for (somewhat) predictable reasons, like insufficient rights, invalid parameters or
        /// because the system's state/business rules wouldn't allow this command to be executed.
        /// </exception>
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
