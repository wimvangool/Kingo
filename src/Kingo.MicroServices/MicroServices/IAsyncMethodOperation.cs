using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an operation where a specific <see cref="IAsyncMethod"/> is being executed.
    /// </summary>
    public interface IAsyncMethodOperation : IMicroProcessorOperation
    {
        /// <summary>
        /// Returns the method that is being executed in this operation.
        /// </summary>
        IAsyncMethod Method
        {
            get;
        }

        /// <summary>
        /// Returns the context of this operation.
        /// </summary>
        MicroProcessorOperationContext Context
        {
            get;
        }
    }
}
