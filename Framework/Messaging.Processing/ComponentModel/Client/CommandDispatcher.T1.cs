using System;
using System.Threading;
using System.Threading.Tasks;

namespace Syztem.ComponentModel.Client
{
    /// <summary>
    /// Represents a command that contains a <see cref="IRequestMessageViewModel">message</see> that serves
    /// as it's execution-parameter.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that serves as the execution-parameter.</typeparam>
    public abstract class CommandDispatcher<TMessage> : CommandDispatcherBase where TMessage : class, IMessage<TMessage>
    {        
        private readonly TMessage _message;        

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
        }

        /// <inheritdoc />
        public TMessage Message
        {
            get { return _message; }
        }        

        #region [====== Execution ======]

        /// <inheritdoc />
        public override void Execute(Guid requestId)
        {                        
            var message = Message.Copy();

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));            

            Processor.Handle(message, msg =>
            {
                try
                {
                    Execute(message);
                }
                catch (Exception exception)
                {
                    OnExecutionFailed(new ExecutionFailedEventArgs(requestId, message, exception));
                    throw;
                }
                OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId, message));    
            });            
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(Guid requestId, CancellationToken token)
        {                        
            var message = Message.Copy();            

            OnExecutionStarted(new ExecutionStartedEventArgs(requestId, message));

            return Processor.HandleAsync(message, msg =>
            {
                try
                {
                    Execute(message);
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
                Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs(requestId, message)));
            }, token);
        }               

        /// <summary>
        /// Executes the command.
        /// </summary>        
        /// <param name="message">The execution-parameter.</param>              
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract void Execute(TMessage message);                

        #endregion
    }
}
