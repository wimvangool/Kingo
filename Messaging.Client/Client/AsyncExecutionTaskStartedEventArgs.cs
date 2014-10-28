namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Arguments for the <see cref="ClientCommand{T}.TaskStarted" /> event.
    /// </summary>
    public class AsyncExecutionTaskStartedEventArgs : EventArgs
    {
        /// <summary>
        /// The task that has been started.
        /// </summary>
        public readonly IAsyncExecutionTask Task;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncExecutionTaskStartedEventArgs" /> class.
        /// </summary>
        /// <param name="task">The task that has been started.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="task"/> is <c>null</c>.
        /// </exception>
        public AsyncExecutionTaskStartedEventArgs(IAsyncExecutionTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            Task = task;
        }
    }
}
