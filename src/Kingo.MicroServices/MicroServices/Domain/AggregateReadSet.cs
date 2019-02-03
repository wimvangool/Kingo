using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Contains a snapshot and a set of events of an aggregate that has been read from a data-store.
    /// </summary>
    [Serializable]
    public sealed class AggregateReadSet        
    {
        /// <summary>
        /// Represents an empty data-set.
        /// </summary>
        public static readonly AggregateReadSet Empty = new AggregateReadSet(Enumerable.Empty<ISnapshotOrEvent>());

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateReadSet" /> class.
        /// </summary>                
        /// <param name="events">A collection of events published by an aggregate.</param> 
        public AggregateReadSet(IEnumerable<ISnapshotOrEvent> events) :
            this(null, events) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateReadSet" /> class.
        /// </summary>        
        /// <param name="snapshot">Snapshot of an aggregate.</param>
        /// <param name="events">A collection of events published by an aggregate.</param>        
        public AggregateReadSet(ISnapshotOrEvent snapshot, IEnumerable<ISnapshotOrEvent> events = null)
        {            
            Snapshot = snapshot;
            Events = (events ?? Enumerable.Empty<ISnapshotOrEvent>()).ToArray();
        }              

        /// <summary>
        /// Snapshot of the aggregate.
        /// </summary>
        public ISnapshotOrEvent Snapshot
        {
            get;
        }

        /// <summary>
        /// Events published by the aggregate.
        /// </summary>
        public IReadOnlyList<ISnapshotOrEvent> Events
        {
            get;
        }        
    }
}
