namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a mode in which a scenario can execute.
    /// </summary>
    public enum ScenarioMode
    {
        /// <summary>
        /// Represents the write-only mode, in which a scenario is used to test message-in/message-out behavior only.
        /// </summary>
        WriteOnly,

        /// <summary>
        /// Represents the read-write mode, in which a scenario is used to test both write (commands/events) and read
        /// (query) logic.
        /// </summary>
        ReadWrite
    }
}
