using System;

namespace Kingo.MicroServices
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
        /// Represents any operation type that is an input-processing operation (command, event or query).
        /// </summary>
        AnyInput = InputStream | Query,        

        /// <summary>
        /// Represents any operation type that are message-processing operations (command or event).
        /// </summary>
        AnyStream = InputStream | OutputStream,

        /// <summary>
        /// Represents all operation types.
        /// </summary>
        All = Query | AnyStream
    }
}
