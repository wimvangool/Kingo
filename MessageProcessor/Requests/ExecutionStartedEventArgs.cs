using System;

namespace YellowFlare.MessageProcessing.Requests
{
    public class ExecutionStartedEventArgs : EventArgs
    {
        public readonly Guid ExecutionId;
        public readonly object Message;

        public ExecutionStartedEventArgs(Guid executionId)
            : this(executionId, null) { }

        public ExecutionStartedEventArgs(Guid executionId, object message)
        {
            ExecutionId = executionId;
            Message = message;
        }
    }
}
