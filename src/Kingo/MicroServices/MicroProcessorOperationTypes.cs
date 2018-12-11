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
        InputMessage = 2,

        /// <summary>
        /// Represents the operation of handling an output-message (event).
        /// </summary>
        OutputMessage = 4,        

        /// <summary>
        /// Represents all operation types that are input-processing operations.
        /// </summary>
        Input = InputMessage | Query,        

        /// <summary>
        /// Represents all operation types that are message-processing operations (command or event).
        /// </summary>
        Message = InputMessage | OutputMessage,

        /// <summary>
        /// Represents all operation types.
        /// </summary>
        All = Query | Message
    }
}
