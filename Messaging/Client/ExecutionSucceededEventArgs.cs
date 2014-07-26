namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// EventArgs for the <see cref="IRequestDispatcher.ExecutionSucceeded" /> event.
    /// </summary>
    public class ExecutionSucceededEventArgs : EventArgs
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
        /// Initializes a new instance of the <see cref="ExecutionSucceededEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>        
        public ExecutionSucceededEventArgs(Guid executionId)
            : this(executionId, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionSucceededEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>
        /// <param name="message">If specified, refers to the message that was sent for the request.</param>
        public ExecutionSucceededEventArgs(Guid executionId, object message)
        {
            ExecutionId = executionId;
            Message = message;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ExecutionCompletedEventArgs" /> based on this instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="ExecutionCompletedEventArgs" /> with the same <see cref="ExecutionId" />
        /// and <see cref="Message" /> as this instance.
        /// </returns>
        public virtual ExecutionCompletedEventArgs ToExecutionCompletedEventArgs()
        {
            return new ExecutionCompletedEventArgs(ExecutionId, Message);
        }
    }
}
