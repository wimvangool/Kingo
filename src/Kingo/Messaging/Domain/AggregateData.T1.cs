using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Contains a snapshot and a set of events that represent the state and state-changes of an aggregate.
    /// </summary>
    public sealed class AggregateData<TKey> where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateData{T}" /> class.
        /// </summary>
        /// <param name="id">Unique identifier of the aggregate.</param>
        /// <param name="snapshot">Snapshot of an aggregate.</param>
        /// <param name="events">A collection of events published by an aggregate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="snapshot"/> is <c>null</c>.
        /// </exception>
        public AggregateData(TKey id, ISnapshot snapshot, IEnumerable<IEvent> events = null)
        {            
            Id = id;
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            Events = events?.ToArray() ?? Enumerable.Empty<IEvent>();
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
        public IEnumerable<IEvent> Events
        {
            get;
        } 
        
        internal AggregateData<TKey> SnapshotOnly() =>
            new AggregateData<TKey>(Id, Snapshot);

        internal AggregateData<TKey> Append(IEnumerable<IEvent> events) =>
            new AggregateData<TKey>(Id, Snapshot, Events.Concat(events).ToArray());
    }
}
