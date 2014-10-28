namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a set of options that defines the behavior of a <see cref="ClientCommand{T}" />.
    /// </summary>
    [Flags]
    public enum ClientCommandOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the command allows multiple executions of the same
        /// request run in parrallel.
        /// </summary>
        AllowParrallelExecutions = 1,

        /// <summary>
        /// Specifies that any previous executions should be cancelled as soon
        /// as the request is executed again.
        /// </summary>
        CancelPreviousOnExecution = 2
    }
}
