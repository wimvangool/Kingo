using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a request - either a command or a query - that can be executed.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Occurs when a new execution of this <see cref="IRequest" /> has started.
        /// </summary>
        event EventHandler<ExecutionStartedEventArgs> ExecutionStarted;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequest" /> was cancelled.
        /// </summary>
        event EventHandler<ExecutionCanceledEventArgs> ExecutionCanceled;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequest" /> has failed.
        /// </summary>
        event EventHandler<ExecutionFailedEventArgs> ExecutionFailed;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequest" /> has succeeded.
        /// </summary>
        event EventHandler<ExecutionSucceededEventArgs> ExecutionSucceeded;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequest" /> has completed; that is,
        /// it was either cancelled, has failed or has succeeded.
        /// </summary>
        event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        /// <summary>
        /// Indicates that <see cref="IsExecuting" /> has changed.
        /// </summary>
        event EventHandler IsExecutingChanged;

        /// <summary>
        /// Indicates whether or not one or more executions for this <see cref="IRequest" /> are running.
        /// </summary>
        bool IsExecuting
        {
            get;
        }
    }
}
