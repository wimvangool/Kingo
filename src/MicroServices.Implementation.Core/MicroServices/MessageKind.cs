namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific role or kind a message that is handled or executed by a <see cref="MicroProcessor"/> can have.
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
        /// Represents a request, carrying parameters for a query.
        /// </summary>
        Request
    }
}
