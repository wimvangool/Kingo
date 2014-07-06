using System;

namespace YellowFlare.MessageProcessing.Requests
{
    public class ExecutionSucceededEventArgs : EventArgs
    {
        public readonly Guid ExecutionId;
        public readonly object Message;

        public ExecutionSucceededEventArgs(Guid executionId)
            : this(executionId, null) { }

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
