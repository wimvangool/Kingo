using System;

namespace Kingo.BuildingBlocks.ComponentModel.Client
{
    /// <summary>
    /// Represents a task that encapsulates the asynchronous execution of a request.
    /// </summary>
    public interface IAsyncExecutionTask : INotifyIsBusy
    {
        /// <summary>
        /// Returns the identifier of this task.
        /// </summary>
        Guid RequestId
        {
            get;
        }

        /// <summary>
        /// Occurs when <see cref="Status" /> changes.
        /// </summary>
        event EventHandler StatusChanged;

        /// <summary>
        /// Returns the status of this task.
        /// </summary>
        AsyncExecutionTaskStatus Status
        {
            get;
        }              

        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The task has already been started.
        /// </exception>
        void Execute();

        /// <summary>
        /// Cancels this task. This method does nothing if the task has already ended.
        /// </summary>        
        void Cancel();
    }
}
