using System;

namespace YellowFlare.MessageProcessing.Requests
{
    public class ExecutionFailedEventArgs : EventArgs
    {
        public readonly Guid ExecutionId;
        public readonly object Message;
        public readonly Exception Exception;

        public ExecutionFailedEventArgs(Guid executionId, Exception exception)
            : this(executionId, null, exception) { }

        public ExecutionFailedEventArgs(Guid executionId, object message, Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            ExecutionId = executionId;
            Message = message;
            Exception = exception;
        }

        public bool Catch<TException>(out TException exception) where TException : Exception
        {
            return (exception = Exception as TException) != null;
        }

        public virtual ExecutionCompletedEventArgs ToExecutionCompletedEventArgs()
        {
            return new ExecutionCompletedEventArgs(ExecutionId, Message);
        }
    }
}
