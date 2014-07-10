using System;
using System.Threading;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Basic implementation of the <see cref="IRequest" /> interface, serving as a base class for all request-types.
    /// </summary>
    public abstract class Request : IRequest
    {
        private readonly RequestContext _requestContext;
        
        internal Request()
        {
            _requestContext = RequestContext.NewContext();
        }

        /// <summary>
        /// Returns the context that is used to publish all events on.
        /// </summary>
        protected RequestContext RequestContext
        {
            get { return _requestContext; }
        }

        #region [====== ExecutionStarted ======]

        /// <inheritdoc />
        public event EventHandler<ExecutionStartedEventArgs> ExecutionStarted;

        /// <summary>
        /// Raises the <see cref="ExecutionStarted" /> event.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionStarted(ExecutionStartedEventArgs e)
        {
            ExecutionStarted.Raise(this, e);

            IncrementExecutionCount();
        }

        #endregion

        #region [====== ExecutionCanceled ======]

        /// <inheritdoc />
        public event EventHandler<ExecutionCanceledEventArgs> ExecutionCanceled;

        /// <summary>
        /// Raises the <see cref="ExecutionCanceled" /> and <see cref="ExecutionCompleted"/> events.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionCanceled(ExecutionCanceledEventArgs e)
        {
            ExecutionCanceled.Raise(this, e);

            OnExecutionCompleted(e.ToExecutionCompletedEventArgs());
        }

        #endregion

        #region [====== ExecutionFailed ======]

        /// <inheritdoc />
        public event EventHandler<ExecutionFailedEventArgs> ExecutionFailed;

        /// <summary>
        /// Raises the <see cref="ExecutionFailed" /> and <see cref="ExecutionCompleted"/> events.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionFailed(ExecutionFailedEventArgs e)
        {
            ExecutionFailed.Raise(this, e);

            OnExecutionCompleted(e.ToExecutionCompletedEventArgs());
        }

        #endregion

        #region [====== ExecutionSucceeded ======]

        event EventHandler<ExecutionSucceededEventArgs> IRequest.ExecutionSucceeded
        {
            add { Add(value); }
            remove { Remove(value); }
        }

        internal abstract void Add(EventHandler<ExecutionSucceededEventArgs> handler);

        internal abstract void Remove(EventHandler<ExecutionSucceededEventArgs> handler);                

        #endregion

        #region [====== ExecutionCompleted ======]

        /// <inheritdoc />
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        /// <summary>
        /// Raises the <see cref="ExecutionCompleted" /> event.
        /// </summary>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnExecutionCompleted(ExecutionCompletedEventArgs e)
        {
            DecrementExecutionCount();

            ExecutionCompleted.Raise(this, e);
        }

        #endregion

        #region [====== IsExecuting ======]

        private int _executionCount;

        /// <inheritdoc />
        public event EventHandler IsExecutingChanged;

        /// <summary>
        /// Raises the <see cref="IsExecutingChanged" /> event.
        /// </summary>                
        protected virtual void OnIsExecutingChanged()
        {
            IsExecutingChanged.Raise(this);
        }

        /// <inheritdoc />
        public bool IsExecuting
        {
            get { return _executionCount > 0; }
        }

        private void IncrementExecutionCount()
        {
            if (Interlocked.Increment(ref _executionCount) == 1)
            {
                OnIsExecutingChanged();
            }
        }

        private void DecrementExecutionCount()
        {            
            if (Interlocked.Decrement(ref _executionCount) == 0)
            {
                OnIsExecutingChanged();
            }
        }       

        #endregion        
    }
}
