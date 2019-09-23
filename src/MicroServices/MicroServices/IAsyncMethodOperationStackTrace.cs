using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a stack trace of operations that are currently being executed
    /// by a <see cref="IMicroProcessor" />.
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
