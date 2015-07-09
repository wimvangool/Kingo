﻿using System;

namespace Syztem.ComponentModel.Client
{
    /// <summary>
    /// EventArgs for the <see cref="IQueryDispatcher{T}.ExecutionSucceeded" /> event.
    /// </summary>
    public class ExecutionSucceededEventArgs<TResult> : ExecutionSucceededEventArgs
    {
        /// <summary>
        /// The result of the associated <see cref="IQueryDispatcher{T}" />.
        /// </summary>
        public readonly TResult Result;		
	
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionSucceededEventArgs" /> class.
        /// </summary>
        /// <param name="requestId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>        
        /// <param name="result">The result of the associated <see cref="IQueryDispatcher{T}" />.</param>
	    public ExecutionSucceededEventArgs(Guid requestId, TResult result)
            : this(requestId, null, result) { }	    
	
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionSucceededEventArgs" /> class.
        /// </summary>
        /// <param name="requestId">Identifier of the execution of the <see cref="IRequestDispatcher" />.</param>
        /// <param name="message">If specified, refers to the message that was sent for the request.</param>
        /// <param name="result">The result of the associated <see cref="IQueryDispatcher{T}" />.</param>
	    public ExecutionSucceededEventArgs(Guid requestId, object message, TResult result)
            : base(requestId, message)
	    {
	        Result = result;
	    }
    }
}