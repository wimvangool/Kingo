using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a command that contains a <see cref="IRequestMessage">message</see> that serves
    /// as it's execution-parameter.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that serves as the execution-parameter.</typeparam>
    public abstract class CommandDispatcher<TMessage> : CommandDispatcherBase where TMessage : class, IRequestMessage
    {        
        private readonly TMessage _message;
        private readonly RequestMessageStateTracker _messageStateTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDispatcher{T}" /> class.
        /// </summary>
        /// <param name="message">The message that serves as the execution-parameter of this command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected CommandDispatcher(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }            
            _message = message;
            _messageStateTracker = new RequestMessageStateTracker(message);
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
        public override void Execute(Guid requestId)
        {                        
            var message = (TMessage) Message.Copy(true);

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));            

            try
            {
                Execute(message, null);
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception));
                throw;
            }            
            OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId, message));
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(Guid requestId, CancellationToken? token)
        {                        
            var message = (TMessage) Message.Copy(true);
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));

            return Start(() =>
            {
                using (var scope = new SynchronizationContextScope(context))
                {                    
                    try
                    {
                        Execute(message, token);
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
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId, message)));
                }
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
        /// <param name="message">The execution-parameter.</param>
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
        protected abstract void Execute(TMessage message, CancellationToken? token);                

        #endregion
    }
}
