namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Indicates which status a certain <see cref="IAsyncExecutionTask" /> has.
    /// </summary>
    public enum AsyncExecutionTaskStatus
    {
        /// <summary>
        /// Indicates the task has not been started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Indicates that the task is still executing.
        /// </summary>
        Running,

        /// <summary>
        /// Indicates that the task was canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// Indicates that an error ocurred while executing the task.
        /// </summary>
        Faulted,

        /// <summary>
        /// Indicates the task was completed succesfully.
        /// </summary>
        Done
    }
}
