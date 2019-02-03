using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{    
    internal sealed class AggregateWriteSet<TKey, TVersion, TSnapshot> : IAggregateWriteSet<TKey, TVersion, TSnapshot>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {        
        internal AggregateWriteSet(TSnapshot snapshot, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events, TVersion? oldVersion = null) :
            this(snapshot, events.OrderBy(@event => @event.Version).ToArray())
        {
            Id = DetermineId(Snapshot, Events);
            NewVersion = DetermineNewVersion(Snapshot, Events);
            OldVersion = oldVersion ?? NewVersion;
        }        

        private AggregateWriteSet(TSnapshot snapshot, IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events)
        {
            Snapshot = snapshot;
            Events = events;            
        }

        private static TKey DetermineId(TSnapshot snapshot, IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events)
        {
            if (snapshot == null)
            {
                if (events.Count == 0)
                {
                    return default;
                }
                return events[0].Id;
            }
            return snapshot.Id;
        }

        private static TVersion DetermineNewVersion(TSnapshot snapshot, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events) =>
            MaxVersion(snapshot?.Version, events.LastOrDefault()?.Version);

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

        #region [====== Id & Version ======]

        public TKey Id
        {
            get;
        }
        
        public TVersion OldVersion
        {
            get;
        }
        
        public TVersion NewVersion
        {
            get;
        }            

        #endregion

        #region [====== Snapshot & Events ======]

        public TSnapshot Snapshot
        {
            get;
        }
        
        public IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Events
        {
            get;
        }

        #endregion
        
        public AggregateReadSet ToReadSet() =>
            new AggregateReadSet(Snapshot, Events);
    }
}
