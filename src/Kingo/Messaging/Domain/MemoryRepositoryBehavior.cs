namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Defines the behavior of a <see cref="MemoryRepository{T, S}" />.
    /// </summary>
    public enum MemoryRepositoryBehavior
    {
        /// <summary>
        /// Indicates that the repository uses snapshots to save and restore aggregates,
        /// effectively making it a state-based repository.
        /// </summary>
        StoreSnapshots,

        /// <summary>
        /// Indicates that the repository uses events to save and restore aggregates,
        /// effectively making it an event-store.
        /// </summary>
        StoreEvents
    }
}
