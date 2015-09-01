using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Client
{
    /// <summary>
    /// Serves as the base-class for the <see cref="QueryDispatcher{T}" /> and <see cref="QueryDispatcher{T, S}"/> classes,
    /// implementing the <see cref="IQueryDispatcher{T}" /> interface.
    /// </summary>
    public abstract class QueryDispatcherBase<TMessageOut> : RequestDispatcher, IQueryDispatcher<TMessageOut> where TMessageOut : class, IMessage<TMessageOut>
    {
        internal QueryDispatcherBase() { }        

        #region [====== ExecutionSucceeded ======]

        /// <summary>
        /// Occurs when an execution of this <see cref="ICommandDispatcher" /> has succeeded.
        /// </summary>
        public event EventHandler<ExecutionSucceededEventArgs<TMessageOut>> ExecutionSucceeded;

        internal override void Add(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded += new EventHandler<ExecutionSucceededEventArgs<TMessageOut>>(handler);
        }

        internal override void Remove(EventHandler<ExecutionSucceededEventArgs> handler)
        {
            ExecutionSucceeded -= new EventHandler<ExecutionSucceededEventArgs<TMessageOut>>(handler);
        }

        /// <summary>
        /// Raises the <see cref="IRequestDispatcher.ExecutionSucceeded" /> and <see cref="RequestDispatcher.ExecutionCompleted"/> events.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionSucceeded(ExecutionSucceededEventArgs<TMessageOut> e)
        {
            ExecutionSucceeded.Raise(this, e);

            OnExecutionCompleted(e.ToExecutionCompletedEventArgs());
        }

        #endregion

        #region [====== Execution ======]

        /// <inheritdoc />
        public abstract TMessageOut Execute(Guid requestId);

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync(Guid requestId)
        {
            return ExecuteAsync(requestId, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task<TMessageOut> ExecuteAsync(Guid requestId, CancellationToken token);                       

        /// <inheritdoc />
        public override IAsyncExecutionTask CreateAsyncExecutionTask()
        {
            return new QueryExecutionTask<TMessageOut>(this);
        }                        

        #endregion
    }
}
