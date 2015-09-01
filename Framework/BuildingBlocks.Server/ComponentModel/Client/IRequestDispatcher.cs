using System;

namespace Kingo.BuildingBlocks.ComponentModel.Client
{
    /// <summary>
    /// Represents a request - either a command or a query - that can be executed.
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Occurs when a new execution of this <see cref="IRequestDispatcher" /> has started.
        /// </summary>
        event EventHandler<ExecutionStartedEventArgs> ExecutionStarted;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequestDispatcher" /> was cancelled.
        /// </summary>
        event EventHandler<ExecutionCanceledEventArgs> ExecutionCanceled;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequestDispatcher" /> has failed.
        /// </summary>
        event EventHandler<ExecutionFailedEventArgs> ExecutionFailed;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequestDispatcher" /> has succeeded.
        /// </summary>
        event EventHandler<ExecutionSucceededEventArgs> ExecutionSucceeded;

        /// <summary>
        /// Occurs when an execution of this <see cref="IRequestDispatcher" /> has completed; that is,
        /// it was either cancelled, has failed or has succeeded.
        /// </summary>
        event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        /// <summary>
        /// Indicates that <see cref="IsExecuting" /> has changed.
        /// </summary>
        event EventHandler IsExecutingChanged;

        /// <summary>
        /// Indicates whether or not one or more executions for this <see cref="IRequestDispatcher" /> are running.
        /// </summary>
        bool IsExecuting
        {
            get;
        }

        /// <summary>
        /// Creates and returns a new <see cref="IAsyncExecutionTask" /> that may execute this request asynchronously.
        /// </summary>
        /// <returns>A new <see cref="IAsyncExecutionTask" />.</returns>
        IAsyncExecutionTask CreateAsyncExecutionTask();
    }
}
