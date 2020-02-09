namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific mode of the unit of work that a <see cref="MicroProcessor" /> uses to store and flush changes.
    /// </summary>
    public enum UnitOfWorkMode
    {
        /// <summary>
        /// Indicates that every <see cref="IChangeTracker">resource manager</see> is flushed immediately when it enlists itself.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates that all enlisted <see cref="IChangeTracker">resource managers</see> are flushed one after the other.
        /// </summary>
        SingleThreaded,

        /// <summary>
        /// Indicates that all <see cref="IChangeTracker">resource managers</see> will be grouped by their
        /// <see cref="IChangeTracker.ResourceId" />, and different groups may be flushed simultaneously using
        /// different threads.
        /// </summary>
        MultiThreaded
    }
}
