namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific mode of the unit of work that a <see cref="MicroProcessor" /> uses to store and flush changes.
    /// </summary>
    public enum UnitOfWorkMode
    {
        /// <summary>
        /// Indicates that every <see cref="IUnitOfWorkResourceManager">resource manager</see> is flushed immediately when it enlists itself.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates that all enlisted <see cref="IUnitOfWorkResourceManager">resource managers</see> are flushed one after the other.
        /// </summary>
        SingleThreaded,

        /// <summary>
        /// Indicates that all <see cref="IUnitOfWorkResourceManager">resource managers</see> will be grouped by their
        /// <see cref="IUnitOfWorkResourceManager.ResourceId" />, and different groups may be flushed simultaneously using
        /// different threads.
        /// </summary>
        MultiThreaded
    }
}
