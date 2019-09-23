namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific operation type performed by a <see cref="IMicroProcessor" />.
    /// </summary>    
    public enum MicroProcessorOperationType
    {
        /// <summary>
        /// Represents an operation in which a message handler is invoked to handle
        /// a message. This operation type is also known as a write operation.
        /// </summary>
        MessageHandlerOperation,

        /// <summary>
        /// Represents an operation in which a query is invoked to return a response.
        /// This operation type is also known as a read operation.
        /// </summary>
        QueryOperation
    }
}
