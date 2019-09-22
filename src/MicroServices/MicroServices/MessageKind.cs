namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific role or kind a message that a message can have.
    /// </summary>
    public enum MessageKind
    {
        /// <summary>
        /// Indicates the kind is yet to be specified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Represents a command.
        /// </summary>
        Command,

        /// <summary>
        /// Represents an event.
        /// </summary>
        Event,

        /// <summary>
        /// Represents the request-message of a query.
        /// </summary>
        QueryRequest,

        /// <summary>
        /// Represents the response-message of a query.
        /// </summary>
        QueryResponse
    }
}
