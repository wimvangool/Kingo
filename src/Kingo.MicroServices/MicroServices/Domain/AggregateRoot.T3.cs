using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IAggregateRoot{T, S, R}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of this aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of this aggregate.</typeparam>
    /// <typeparam name="TSnapshot">Type of the snapshot of this aggregate.</typeparam>
    public abstract class AggregateRoot<TKey, TVersion, TSnapshot> : AggregateRoot<TKey, TVersion>, IAggregateRoot<TKey, TVersion, TSnapshot>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot" /> class.
        /// </summary>
        /// <inheritdoc />
        protected AggregateRoot(AggregateRoot parent, ISnapshotOrEvent<TKey, TVersion> @event) :
            base(parent, @event) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot" /> class.
        /// </summary>
        /// <inheritdoc />
        protected AggregateRoot(IEventBus eventBus, ISnapshotOrEvent<TKey, TVersion> snapshotOrEvent, bool isNewAggregate) :
            base(eventBus, snapshotOrEvent, isNewAggregate) { }

        TSnapshot IAggregateRoot<TKey, TVersion, TSnapshot>.TakeSnapshot() =>
            TakeSnapshot();

        /// <summary>
        /// When overridden, creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        /// <exception cref="NotSupportedException">
        /// This aggregate does not support snapshots.
        /// </exception>
        protected abstract TSnapshot TakeSnapshot();
    }
}
