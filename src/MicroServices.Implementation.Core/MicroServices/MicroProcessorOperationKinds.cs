using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the kinds of operations a processor can execute (i.e. internal or external).
    /// </summary>
    [Flags]
    public enum MicroProcessorOperationKinds
    {
        /// <summary>
        /// Represents no specific kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents an operation that was triggered by an external message or request. Root operations always
        /// represent the first operation on the <see cref="IAsyncMethodOperationStackTrace" />.
        /// </summary>
        RootOperation = 1,

        /// <summary>
        /// Represents an operation that was triggered by another operation.
        /// </summary>
        BranchOperation = 2,

        /// <summary>
        /// Represents any kind of operation.
        /// </summary>
        Any = RootOperation | BranchOperation
    }
}
