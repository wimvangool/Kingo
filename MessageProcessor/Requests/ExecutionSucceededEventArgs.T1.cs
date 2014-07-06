using System;

namespace YellowFlare.MessageProcessing.Requests
{
    public class ExecutionSucceededEventArgs<TResult> : ExecutionSucceededEventArgs
    {
        public readonly TResult Result;		
	
	    public ExecutionSucceededEventArgs(Guid executionId, TResult result)
            : this(executionId, null, result) { }	    
	
	    public ExecutionSucceededEventArgs(Guid executionId, object message, TResult result)
            : base(executionId, message)
	    {
	        Result = result;
	    }
    }
}
