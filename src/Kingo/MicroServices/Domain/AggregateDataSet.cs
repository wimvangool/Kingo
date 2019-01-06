using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Contains a snapshot and a set of events that represent the state and state-changes of an aggregate.
    /// </summary>
    [Serializable]
    public class AggregateDataSet        
    {             
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDataSet" /> class.
        /// </summary>        
        /// <param name="snapshot">Snapshot of an aggregate.</param>
        /// <param name="events">A collection of events published by an aggregate.</param>        
        public AggregateDataSet(ISnapshotOrEvent snapshot = null, IEnumerable<ISnapshotOrEvent> events = null)
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
