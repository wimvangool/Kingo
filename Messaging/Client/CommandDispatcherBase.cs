using System.ComponentModel.Messaging.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Serves as the base-class for the <see cref="CommandDispatcher" /> and <see cref="CommandDispatcher{T}"/> classes,
    /// implementing the <see cref="ICommandDispatcher" /> interface.
    /// </summary>
    public abstract class CommandDispatcherBase : RequestDispatcher, ICommandDispatcher
    {
        internal CommandDispatcherBase() { }

        #region [====== ExecutionSucceeded ======]

        /// <summary>
        /// Occurs when an execution of this <see cref="ICommandDispatcher" /> has succeeded.
        /// </summary>
        public event EventHandler<ExecutionSucceededEventArgs> ExecutionSucceeded;

        internal override void Add(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded += handler;
        }

        internal override void Remove(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded -= handler;
        }

        /// <summary>
        /// Raises the <see cref="IRequestDispatcher.ExecutionSucceeded" /> and <see cref="RequestDispatcher.ExecutionCompleted"/> events.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionSucceeded(ExecutionSucceededEventArgs e)
        {
            ExecutionSucceeded.Raise(this, e);

            OnExecutionCompleted(e.ToExecutionCompletedEventArgs());
        }

        #endregion

        #region [====== Execution ======]

        /// <inheritdoc />
        public abstract void Execute(Guid requestId);

        /// <inheritdoc />
        public Task ExecuteAsync(Guid requestId)
        {
            return ExecuteAsync(requestId, null);
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync(Guid requestId, CancellationToken? token);

        /// <inheritdoc />
        public override IAsyncExecutionTask CreateAsyncExecutionTask()
        {
            return new CommandExecutionTask(this);
        }

        #endregion
    }
}
