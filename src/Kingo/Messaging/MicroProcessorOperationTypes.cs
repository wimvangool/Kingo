using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Defines all operation types of a <see cref="MicroProcessor" />.
    /// </summary>
    [Flags]
    public enum MicroProcessorOperationTypes
    {
        /// <summary>
        /// Represents no operation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a query-operation.
        /// </summary>
        Query = 1,

        /// <summary>
        /// Represents the operation of handling an input-message (command or event).
        /// </summary>
        InputStream = 2,

        /// <summary>
        /// Represents the operation of handling an output-message (event).
        /// </summary>
        OutputStream = 4,        

        /// <summary>
        /// Represents all operation types that are input-processing operations.
        /// </summary>
        Input = InputStream | Query,        

        /// <summary>
        /// Represents all operation types that are stream-processing operations.
        /// </summary>
        Stream = InputStream | OutputStream,

        /// <summary>
        /// Represents all operation types.
        /// </summary>
        All = Query | Stream
    }
}
