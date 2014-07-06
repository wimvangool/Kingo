using System;

namespace YellowFlare.MessageProcessing.Requests
{
    public class ExecutionCompletedEventArgs : EventArgs
    {
        public readonly Guid ExecutionId;
        public readonly object Message;

        public ExecutionCompletedEventArgs(Guid executionId)
            : this(executionId, null) { }

        public ExecutionCompletedEventArgs(Guid executionId, object message)
        {
            ExecutionId = executionId;
            Message = message;
        }
    }
}
