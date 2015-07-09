﻿using System;
using System.Threading;
using Syztem.Resources;

namespace Syztem.ComponentModel.Client
{
    /// <summary>
    /// Provides a base-class implementation of the <see cref="IAsyncExecutionTask" /> interface.
    /// </summary>
    /// <typeparam name="TDispatcher">Type of the encapsulated <see cref="IRequestDispatcher" />.</typeparam>
    public abstract class AsyncExecutionTask<TDispatcher> : PropertyChangedBase, IAsyncExecutionTask where TDispatcher : class, IRequestDispatcher
    {
        private readonly Guid _requestId;
        private readonly Lazy<CancellationTokenSource> _cancellationTokenSource;                
        private AsyncExecutionTaskStatus _status;
        private EventHandler _isBusyChangedHandlers;

        internal AsyncExecutionTask()
        {
            _requestId = Guid.NewGuid();
            _cancellationTokenSource = new Lazy<CancellationTokenSource>(() => new CancellationTokenSource());            
        }

        /// <inheritdoc />
        public Guid RequestId
        {
            get { return _requestId; }
        }

        #region [====== Dispatcher ======]

        /// <summary>
        /// Returns the dispatcher that will dispatch the request when <see cref="IAsyncExecutionTask.Execute()" /> is called.
        /// </summary>
        protected abstract TDispatcher Dispatcher
        {
            get;
        }

        private void OnTaskStarting()
        {
            Dispatcher.ExecutionCanceled += HandleRequestCanceled;
            Dispatcher.ExecutionFailed += HandleExecutionFailed;
            Dispatcher.ExecutionSucceeded += HandleExecutionSucceeded;
        }               

        private void HandleRequestCanceled(object sender, ExecutionCanceledEventArgs e)
        {
            if (e.RequestId.Equals(RequestId))
            {
                Status = AsyncExecutionTaskStatus.Canceled;

                OnTaskEnded();
            }
        }

        private void HandleExecutionFailed(object sender, ExecutionFailedEventArgs e)
        {
            if (e.RequestId.Equals(RequestId))
            {
                Status = AsyncExecutionTaskStatus.Faulted;

                OnTaskEnded();
            }
        }

        private void HandleExecutionSucceeded(object sender, ExecutionSucceededEventArgs e)
        {
            if (e.RequestId.Equals(RequestId))
            {
                Status = AsyncExecutionTaskStatus.Done;               

                OnTaskEnded();
            }
        }        

        private void OnTaskEnded()
        {
            Dispatcher.ExecutionSucceeded -= HandleExecutionSucceeded;
            Dispatcher.ExecutionFailed -= HandleExecutionFailed;
            Dispatcher.ExecutionCanceled -= HandleRequestCanceled;
            
            if (_cancellationTokenSource.IsValueCreated)
            {
                _cancellationTokenSource.Value.Dispose();
            }
        }

        #endregion

        #region [====== IsBusyIndicator ======]

        event EventHandler INotifyIsBusy.IsBusyChanged
        {
            add { _isBusyChangedHandlers += value; }
            remove { _isBusyChangedHandlers -= value; }
        }

        /// <summary>
        /// Raises the <see cref="INotifyIsBusy.IsBusyChanged" /> event.
        /// </summary>
        protected virtual void OnIsBusyChanged()
        {
            _isBusyChangedHandlers.Raise(this);

            NotifyOfPropertyChange("IsBusy");
        }

        bool INotifyIsBusy.IsBusy
        {
            get { return Status == AsyncExecutionTaskStatus.Running; }
        }

        #endregion

        #region [====== Status ======]

        /// <inheritdoc />
        public event EventHandler StatusChanged;

        /// <summary>
        /// Raises the <see cref="StatusChanged" /> event.
        /// </summary>
        protected virtual void OnStatusChanged()
        {
            StatusChanged.Raise(this);

            OnIsBusyChanged();
            NotifyOfPropertyChange(() => Status);
        }

        /// <inheritdoc />
        public AsyncExecutionTaskStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    _status = value;

                    OnStatusChanged();
                }
            }
        }

        #endregion        

        #region [====== Execution ======]

        void IAsyncExecutionTask.Execute()
        {
            if (Status == AsyncExecutionTaskStatus.NotStarted)
            {
                OnTaskStarting();

                Execute(_cancellationTokenSource.Value.Token);

                Status = AsyncExecutionTaskStatus.Running;
                return;
            }
            throw NewTaskAlreadyStartedOrCanceledException();
        }        

        /// <summary>
        /// Executes the request, enabling it to be canceled and to report progress.
        /// </summary>
        /// <param name="token">
        /// Token that can be used to cancel the task.
        /// </param>        
        protected abstract void Execute(CancellationToken token);

        /// <inheritdoc />
        public void Cancel()
        {
            if (Status == AsyncExecutionTaskStatus.NotStarted)
            {
                Status = AsyncExecutionTaskStatus.Canceled;
            }
            else if (Status == AsyncExecutionTaskStatus.Running)
            {
                _cancellationTokenSource.Value.Cancel();
            }
        }

        private static Exception NewTaskAlreadyStartedOrCanceledException()
        {
            return new InvalidOperationException(ExceptionMessages.AsyncExecutionTask_TaskAlreadyStarted);
        }

        #endregion
    }
}