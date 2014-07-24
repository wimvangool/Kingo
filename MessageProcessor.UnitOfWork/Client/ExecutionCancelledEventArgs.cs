using System;

namespace YellowFlare.MessageProcessing.Client
{
    /// <summary>
    /// EventArgs for the <see cref="IRequestDispatcher.ExecutionCanceled" /> event.
    /// </summary>
    public class ExecutionCanceledEventArgs : EventArgs
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
        /// The exception that was thrown while executing the <see cref="IRequestDispatcher" />.
        /// </summary>
        public readonly OperationCanceledException Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionCanceledEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>        
        /// <param name="exception">The exception that was thrown while executing the <see cref="IRequestDispatcher" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        public ExecutionCanceledEventArgs(Guid executionId, OperationCanceledException exception)
            : this(executionId, null, exception) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionCanceledEventArgs" /> class.
        /// </summary>
        /// <param name="executionId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>
        /// <param name="message">If specified, refers to the message that was sent for the request.</param>
        /// <param name="exception">The exception that was thrown while executing the <see cref="IRequestDispatcher" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        public ExecutionCanceledEventArgs(Guid executionId, object message, OperationCanceledException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            ExecutionId = executionId;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ExecutionCompletedEventArgs" /> from this instance.
        /// </summary>
        /// <returns>A new <see cref="ExecutionCompletedEventArgs" />.</returns>
        public virtual ExecutionCompletedEventArgs ToExecutionCompletedEventArgs()
        {
            return new ExecutionCompletedEventArgs(ExecutionId, Message);
        }
    }
}
