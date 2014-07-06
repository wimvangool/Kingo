using System;

namespace YellowFlare.MessageProcessing.Requests
{
    public class ExecutionCanceledEventArgs : EventArgs
    {
        public readonly Guid ExecutionId;
        public readonly object Message;
        public readonly OperationCanceledException Exception;

        public ExecutionCanceledEventArgs(Guid executionId, OperationCanceledException exception)
            : this(executionId, null, exception) { }

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

        public virtual ExecutionCompletedEventArgs ToExecutionCompletedEventArgs()
        {
            return new ExecutionCompletedEventArgs(ExecutionId, Message);
        }
    }
}
