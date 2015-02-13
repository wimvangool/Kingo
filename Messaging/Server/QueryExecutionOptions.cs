namespace System.ComponentModel.Server
{
    /// <summary>
    /// Declares bitwise options that are used to configure how a specific <see cref="IQuery{T, S}" />
    /// is to be executed by a <see cref="IMessageProcessor" />.
    /// </summary>
    [Flags]
    public enum QueryExecutionOptions
    {
        /// <summary>
        /// Specifies that all cache-related functionality is to be disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the processor is allowed to consult the cache for
        /// any previously stored result of the query.
        /// </summary>
        AllowCacheRead = 1,

        /// <summary>
        /// Specifies that the processor is allowed to store any retrieved result
        /// into the cache after the query has been executed.
        /// </summary>
        AllowCacheWrite = 2,

        /// <summary>
        /// Allows both reading from and writing to the cache.
        /// </summary>
        Default = AllowCacheRead | AllowCacheWrite
    }
}
