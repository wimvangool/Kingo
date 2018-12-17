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
        InputMessageHandler = 2,

        /// <summary>
        /// Represents the operation of handling an output-message (event).
        /// </summary>
        OutputMessageHandler = 4,        

        /// <summary>
        /// Represents all operation types that are input-processing operations.
        /// </summary>
        Input = InputMessageHandler | Query,        

        /// <summary>
        /// Represents all operation types that are message-processing operations (command or event).
        /// </summary>
        MessageHandler = InputMessageHandler | OutputMessageHandler,

        /// <summary>
        /// Represents all operation types.
        /// </summary>
        All = Query | MessageHandler
    }
}
