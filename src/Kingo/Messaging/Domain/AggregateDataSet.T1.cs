using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Contains a snapshot and a set of events that represent the state and state-changes of an aggregate.
    /// </summary>
    public sealed class AggregateDataSet<TKey> : IAggregateRootFactory where TKey : struct, IEquatable<TKey>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDataSet{TKey}" /> class.
        /// </summary>
        /// <param name="id">Unique identifier of the aggregate.</param>
        /// <param name="snapshot">Snapshot of an aggregate.</param>
        /// <param name="events">A collection of events published by an aggregate.</param>        
        public AggregateDataSet(TKey id, ISnapshot snapshot, IEnumerable<IEvent> events = null)
        {            
            Id = id;
            Snapshot = snapshot;
            Events = (events ?? Enumerable.Empty<IEvent>()).ToArray();
        }

        /// <summary>
        /// Unique identifier of the aggregate.
        /// </summary>
        public TKey Id
        {
            get;
        }

        /// <summary>
        /// Snapshot of an aggregate.
        /// </summary>
        public ISnapshot Snapshot
        {
            get;
        }

        /// <summary>
        /// A collection of events published by an aggregate.
        /// </summary>
        public IReadOnlyList<IEvent> Events
        {
            get;
        }        

        internal AggregateDataSet<TKey> Append(AggregateDataSet<TKey> dataSet) =>
            new AggregateDataSet<TKey>(Id, dataSet.Snapshot ?? Snapshot, Events.Concat(dataSet.Events));

        /// <inheritdoc />
        public TAggregate RestoreAggregate<TAggregate>() where TAggregate : IAggregateRoot =>
            UpdateToLatestVersion().RestoreAggregateCore<TAggregate>();

        private TAggregate RestoreAggregateCore<TAggregate>() where TAggregate : IAggregateRoot  =>
            AggregateRootFactory.FromDataSet(Snapshot, Events).RestoreAggregate<TAggregate>();        

        /// <summary>
        /// Updates all data (snapshots and events) to the latest version and returns the result in the form of a new data object.
        /// </summary>
        /// <returns>A new data object containing the latest versions of both snapshot and events.</returns>
        public AggregateDataSet<TKey> UpdateToLatestVersion() =>
            new AggregateDataSet<TKey>(Id, Snapshot?.UpdateToLatestVersion(), Events.Select(@event => @event.UpdateToLatestVersion()));

        /// <inheritdoc />
        public override string ToString() =>
            AggregateRootFactory.FromDataSet(Snapshot, Events).ToString();
    }
}
