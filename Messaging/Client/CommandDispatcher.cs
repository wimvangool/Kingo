using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{    
    /// <summary>
    /// Represents a command that has no execution-parameter(s).
    /// </summary>
    public abstract class CommandDispatcher : CommandDispatcherBase, IRequestMessageDispatcher
    {                       
        #region [====== Execution ======]

        /// <inheritdoc />
        public override void Execute(Guid requestId)
        {                        
            OnExecutionStarted(new ExecutionStartedEventArgs(requestId));            

            Processor.Handle(new RequestMessage(this), msg =>
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

            return Processor.HandleAsync(new RequestMessage(this), msg =>
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
