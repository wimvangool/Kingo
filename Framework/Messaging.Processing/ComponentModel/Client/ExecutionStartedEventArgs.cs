namespace System.ComponentModel.Client
{
    /// <summary>
    /// EventArgs for the <see cref="IRequestDispatcher.ExecutionStarted" /> event.
    /// </summary>
    public class ExecutionStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Identifier of the execution of the <see cref="IRequestDispatcher" />.
        /// </summary>
        public readonly Guid RequestId;

        /// <summary>
        /// If specified, refers to the message that was sent for the request.
        /// </summary>
        public readonly object Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionStartedEventArgs" /> class.
        /// </summary>
        /// <param name="requestId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>
        public ExecutionStartedEventArgs(Guid requestId)
            : this(requestId, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionStartedEventArgs" /> class.
        /// </summary>
        /// <param name="requestId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>
        /// <param name="message">If specified, refers to the message that was sent for the request.</param>
        public ExecutionStartedEventArgs(Guid requestId, object message)
        {
            RequestId = requestId;
            Message = message;
        }
    }
}
