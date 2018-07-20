using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a stack of operations as they are currently being performed by a <see cref="IMicroProcessor" />.
    /// </summary>
    public interface IStackTrace : IReadOnlyList<MicroProcessorOperation>
    {
        /// <summary>
        /// Indicates whether or not any operation is being performed.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Returns the current operation that is being performed.
        /// </summary>
        MicroProcessorOperation CurrentOperation
        {
            get;
        }
    }
}
