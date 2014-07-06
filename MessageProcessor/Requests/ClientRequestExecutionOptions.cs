using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a set of options that defines the behavior of a <see cref="ClientRequest{T, S}" />.
    /// </summary>
    [Flags]
    public enum ClientRequestExecutionOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the request is allowed to have nested (in case of synchronous) or
        /// parrallel (in case of asynchronous) executions.
        /// </summary>
        AllowMultipleExecutions = 1,

        /// <summary>
        /// Forces the request to execute synchronously.
        /// </summary>
        ExecuteSynchronously = 2
    }
}
