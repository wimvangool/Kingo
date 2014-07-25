namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// EventArgs for the <see cref="IRequestDispatcher.ExecutionCompleted" /> event.
    /// </summary>
    public class ExecutionCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Identifier of the execution of the <see cref="IRequestDispatcher" />.
        /// </summary>
        public readonly Guid ExecutionId;

        /// <summary>
        /// If specified, refers to the message that was sent for the request.
        /// </summary>
        public readonly object Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>        
        public ExecutionCompletedEventArgs(Guid executionId)
            : this(executionId, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>
        /// <param name="message">If specified, refers to the message that was sent for the request.</param>
        public ExecutionCompletedEventArgs(Guid executionId, object message)
        {
            ExecutionId = executionId;
            Message = message;
        }
    }
}
