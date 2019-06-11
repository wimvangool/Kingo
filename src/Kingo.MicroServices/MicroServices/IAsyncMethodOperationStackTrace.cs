using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a stack trace of operations that are currently being executed
    /// by a <see cref="MicroProcessor" />.
    /// </summary>
    public interface IAsyncMethodOperationStackTrace : IReadOnlyList<IAsyncMethodOperation>
    {
        /// <summary>
        /// Returns the operation that is currently being executed.
        /// </summary>
        IAsyncMethodOperation CurrentOperation
        {
            get;
        }
    }
}
