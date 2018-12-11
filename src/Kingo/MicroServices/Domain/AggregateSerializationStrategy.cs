using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Indicates which strategy a repository uses to serialize and deserialize its aggregates.
    /// </summary>
    [Flags]
    public enum AggregateSerializationStrategy
    {
        /// <summary>
        /// Unspecified strategy.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Indicates that the repository takes and stores snapshots of an aggregate.
        /// </summary>
        Snapshots = 1,

        /// <summary>
        /// Indicates that the repository stores all events that are published by the aggregate.
        /// </summary>
        Events = 2,

        /// <summary>
        /// Indicates that the repository stores both snapshots and events of an aggregate,
        /// where snapshots are used as an restore-optimization technique.
        /// </summary>
        EventsAndSnapshots = Snapshots | Events
    }
}
