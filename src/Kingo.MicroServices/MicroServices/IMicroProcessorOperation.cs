using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an operation of a <see cref="IMicroProcessor" />.
    /// </summary>
    public interface IMicroProcessorOperation
    {        
        /// <summary>
        /// Returns the message that is being handled or executed. Returns <c>null</c>
        /// if this operation refers to the execution of a <see cref="IQuery{TResponse}"/>.        
        /// </summary>
        IMessage Message
        {
            get;
        }  
        
        /// <summary>
        /// Returns the token that indicates whether or not cancellation of the operation is requested.
        /// </summary>
        CancellationToken Token
        {
            get;
        }

        /// <summary>
        /// Indicates whether this operation is invoking a message handler or executing a query.
        /// </summary>
        MicroProcessorOperationType Type
        {
            get;
        }

        /// <summary>
        /// Indicates whether this operation is an internal or external operation.
        /// </summary>
        MicroProcessorOperationKinds Kind
        {
            get;
        }
    }
}
