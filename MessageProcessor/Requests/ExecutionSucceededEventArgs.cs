using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// EventArgs for the <see cref="IRequest.ExecutionSucceeded" /> event.
    /// </summary>
    public class ExecutionSucceededEventArgs : EventArgs
    {
        /// <summary>
        /// Identifier of the execution of the <see cref="IRequest" />.
        /// </summary>
        public readonly Guid ExecutionId;

        /// <summary>
        /// If specified, refers to the message that was sent for the request.
        /// </summary>
        public readonly object Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionSucceededEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequest" />.</param>        
        public ExecutionSucceededEventArgs(Guid executionId)
            : this(executionId, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionSucceededEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequest" />.</param>
        /// <param name="message">If specified, refers to the message that was sent for the request.</param>
        public ExecutionSucceededEventArgs(Guid executionId, object message)
        {
            ExecutionId = executionId;
            Message = message;
        }

        public virtual ExecutionCompletedEventArgs ToExecutionCompletedEventArgs()
        {
            return new ExecutionCompletedEventArgs(ExecutionId, Message);
        }
    }
}
