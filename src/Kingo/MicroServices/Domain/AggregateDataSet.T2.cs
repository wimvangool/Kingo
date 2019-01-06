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
        internal AggregateDataSet(ISnapshotOrEvent<TKey, TVersion> snapshot, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events, TVersion? oldVersion = null)            
        {            
            Snapshot = snapshot;
            Events = events.OrderBy(@event => @event.Version).ToArray();
            OldVersion = oldVersion ?? NewVersion;            
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
        /// Identifier of the aggregate.
        /// </summary>
        public TKey Id
        {
            get
            {
                if (Snapshot == null)
                {
                    if (Events.Count == 0)
                    {
                        return default;
                    }
                    return Events[0].Id;
                }
                return Snapshot.Id;
            }
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
        
        /// <summary>
        /// Converts this versioned data set to a regular data-set.
        /// </summary>
        /// <returns>A new instance of a regular data-set, containing the same snapshot and events.</returns>
        public AggregateDataSet ToDataSet() =>
            new AggregateDataSet(Snapshot, Events);
    }
}
