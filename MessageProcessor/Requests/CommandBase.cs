using System;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Serves as the base-class for the <see cref="Command" /> and <see cref="Command{T}"/> classes,
    /// implementing the <see cref="ICommand" /> interface.
    /// </summary>
    public abstract class CommandBase : Request, ICommand
    {
        internal CommandBase() { }

        #region [====== ExecutionSucceeded ======]

        /// <summary>
        /// Occurs when an execution of this <see cref="ICommand" /> has succeeded.
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
        /// Raises the <see cref="IRequest.ExecutionSucceeded" /> and <see cref="Request.ExecutionCompleted"/> events.
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
        public abstract void Execute();

        /// <inheritdoc />
        public async Task ExecuteAsync()
        {
            await ExecuteAsync(null);
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync(CancellationToken? token);

        #endregion
    }
}
