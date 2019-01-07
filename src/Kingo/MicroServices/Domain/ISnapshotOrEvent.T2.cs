using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a snapshot of an aggregate, or an event published by an aggregate,
    /// that contains its key and version.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam> 
    public interface ISnapshotOrEvent<TKey, TVersion> : ISnapshotOrEvent
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Key or identifier of the aggregate.
        /// </summary>
        TKey Id
        {
            get;
        }

        /// <summary>
        /// Version of the aggregate at the time this snapshot was created or event was published.
        /// </summary>
        TVersion Version
        {
            get;
        }

        /// <summary>
        /// Restores and returns the aggregate this snapshot or event originated from.
        /// </summary>
        /// <param name="eventBus">The event bus to which all events published by the aggregate will be written to.</param>
        /// <returns>The restored aggregate.</returns>        
        /// <exception cref="NotSupportedException">
        /// This instance represents an event that cannot be used to restore its associated aggregate.
        /// </exception>        
        IAggregateRoot<TKey, TVersion> RestoreAggregate(IEventBus eventBus = null);
    }
}
