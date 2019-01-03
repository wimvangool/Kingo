using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Contains a snapshot and a set of events that represent the state and state-changes of an aggregate,
    /// plus the version of the aggregate that it had before it published the events. This data-set is used
    /// to write new versions and/or events of an aggregate to a data-store.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>    
    public sealed class AggregateDataSet<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {        
        internal AggregateDataSet(TKey id, ISnapshotOrEvent<TKey, TVersion> snapshot, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events, TVersion? oldVersion = null)            
        {
            Id = id;
            Snapshot = snapshot;
            Events = events.OrderBy(@event => @event.Version).ToArray();
            OldVersion = oldVersion ?? NewVersion;            
        }

        /// <summary>
        /// Identifier of the aggregate,
        /// </summary>
        public TKey Id
        {
            get;
        }

        /// <summary>
        /// Snapshot that was taken from the aggregate (or <c>null</c> if none was taken).
        /// </summary>
        public ISnapshotOrEvent<TKey, TVersion> Snapshot
        {
            get;
        }

        /// <summary>
        /// Events that were published by the aggregate.
        /// </summary>
        public IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Events
        {
            get;
        }

        /// <summary>
        /// The version of the aggregate that it had when it was read from the data-store. It
        /// has the default value for new (added) aggregates. 
        /// </summary>
        /// <remarks>
        /// This value can be used to throw a <see cref="ConcurrencyException" /> from the repository
        /// if this value does not match the value in the data-store, implying that the aggregate
        /// was updated in another transaction while the current transaction was running.
        /// </remarks>
        public TVersion OldVersion
        {
            get;
        }

        /// <summary>
        /// The current version of the aggregate.
        /// </summary>
        public TVersion NewVersion =>
            MaxVersion(Snapshot?.Version, Events.LastOrDefault()?.Version);

        private static TVersion MaxVersion(TVersion? snapshotVersion, TVersion? lastEventVersion)
        {
            if (snapshotVersion.HasValue)
            {
                if (lastEventVersion.HasValue)
                {
                    return MaxVersion(snapshotVersion.Value, lastEventVersion.Value);
                }
                return snapshotVersion.Value;
            }
            if (lastEventVersion.HasValue)
            {
                return lastEventVersion.Value;
            }
            return default;
        }

        private static TVersion MaxVersion(TVersion snapshotVersion, TVersion lastEventVersion) =>
            snapshotVersion.CompareTo(lastEventVersion) < 0 ? lastEventVersion : snapshotVersion;

        #region [====== RestoreAggregate ======]

        /// <summary>
        /// Restores and returns a new instance of the desired aggregate.
        /// </summary>
        /// <typeparam name="TAggregate">Type of the aggregate to restore.</typeparam>
        /// <param name="eventBus">The event bus to which all events published by the aggregate will be written to.</param>
        /// <returns>The restored aggregate.</returns>
        /// <exception cref="InvalidOperationException">
        /// The data-set contains no snapshot, nor any events to restore the aggregate with.
        /// </exception>
        public TAggregate RestoreAggregate<TAggregate>(IEventBus eventBus = null)
            where TAggregate : class, IAggregateRoot<TKey, TVersion>
        {
            if (Snapshot == null)
            {
                if (Events.Count == 0)
                {
                    throw NewMissingAggregateFactoryException(typeof(TAggregate));
                }
                return RestoreAggregate<TAggregate>(eventBus, Events[0], Events.Skip(1));
            }
            return RestoreAggregate<TAggregate>(eventBus, Snapshot, Events);
        }

        private static TAggregate RestoreAggregate<TAggregate>(IEventBus eventBus, ISnapshotOrEvent<TKey, TVersion> factory, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events)
            where TAggregate : class, IAggregateRoot<TKey, TVersion>
        {
            var aggregate = factory.RestoreAggregate(eventBus);
            aggregate.LoadFromHistory(events);
            return Convert<TAggregate>(aggregate);
        }

        private static TAggregate Convert<TAggregate>(IAggregateRoot<TKey, TVersion> aggregate)
            where TAggregate : class, IAggregateRoot<TKey, TVersion>
        {
            try
            {
                return (TAggregate)aggregate;
            }
            catch (InvalidCastException)
            {
                throw NewUnexpectedAggregateTypeException(typeof(TAggregate), aggregate);
            }
        }

        private static Exception NewMissingAggregateFactoryException(Type aggregateType)
        {
            var messageFormat = ExceptionMessages.AggregateDataSet_CannotRestoreAggregate;
            var message = string.Format(messageFormat, aggregateType.FriendlyName());
            return new InvalidOperationException(message);
        }

        private static Exception NewUnexpectedAggregateTypeException(Type aggregateType, object aggregate)
        {
            var messageFormat = ExceptionMessages.AggregateDataSet_UnexpectedAggregateType;
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), aggregate.GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
