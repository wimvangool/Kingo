using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Represents the strategy a repository uses to serialize and deserialize its aggregates.
    /// </summary>    
    public sealed class SerializationStrategy
    {
        #region [====== Implementation ======]

        private abstract class Implementation
        {
            public AggregateDataSet<TKey, TVersion> Serialize<TKey, TVersion>(IAggregateRoot<TKey, TVersion> aggregate, TVersion? oldVersion, int eventsSinceLastSnapshot)
                where TKey : struct, IEquatable<TKey>
                where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            {
                var id = aggregate.Id;               
                var events = GetEvents(aggregate.Commit());
                var snapshot = GetSnapshot(aggregate, events.Count + eventsSinceLastSnapshot);
                return new AggregateDataSet<TKey, TVersion>(id, snapshot, events, oldVersion);
            }

            protected virtual IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> GetEvents<TKey, TVersion>(IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events)
                where TKey : struct, IEquatable<TKey>
                where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            {
                return events;
            }

            protected virtual ISnapshotOrEvent<TKey, TVersion> GetSnapshot<TKey, TVersion>(IAggregateRoot<TKey, TVersion> aggregate, int eventsSinceLastSnapshot)
                where TKey : struct, IEquatable<TKey>
                where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            {
                return aggregate.TakeSnapshot();
            }            
        }

        #endregion

        #region [====== UseSnapshotsImplementation ======]

        private sealed class UseSnapshotsImplementation : Implementation
        {
            public override string ToString() =>
                nameof(UseSnapshots);

            protected override IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> GetEvents<TKey, TVersion>(IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events) =>
                new ISnapshotOrEvent<TKey, TVersion>[0];
        }

        #endregion

        #region [====== UseEventsImplementation ======]

        private sealed class UseEventsImplementation : Implementation
        {
            private readonly int _eventsPerSnapshot;

            public UseEventsImplementation(int eventsPerSnapshot)
            {
                if (eventsPerSnapshot < 0)
                {
                    throw NewInvalidEventsPerSnapshotException(eventsPerSnapshot);
                }
                _eventsPerSnapshot = eventsPerSnapshot;
            }            

            public override string ToString() =>
                $"{nameof(UseEvents)} (EventsPerSnapshot = {_eventsPerSnapshot})";

            protected override ISnapshotOrEvent<TKey, TVersion> GetSnapshot<TKey, TVersion>(IAggregateRoot<TKey, TVersion> aggregate, int eventsSinceLastSnapshot)
            {
                if (0 < _eventsPerSnapshot && _eventsPerSnapshot <= eventsSinceLastSnapshot)
                {
                    return base.GetSnapshot(aggregate, eventsSinceLastSnapshot);
                }
                return null;
            }

            private static Exception NewInvalidEventsPerSnapshotException(int eventsPerSnapshot)
            {
                var messageFormat = ExceptionMessages.SerializationStrategy_InvalidEventsPerSnapshot;
                var message = string.Format(messageFormat, eventsPerSnapshot);
                return new ArgumentOutOfRangeException(nameof(eventsPerSnapshot), message);
            }
        }

        #endregion

        private readonly Implementation _implementation;

        private SerializationStrategy(Implementation implementation)
        {
            _implementation = implementation;
        }

        /// <inheritdoc />
        public override string ToString() =>
            _implementation.ToString();        

        internal int AddAggregateTo<TKey, TVersion>(ICollection<AggregateDataSet<TKey, TVersion>> changeSet, IAggregateRoot<TKey, TVersion> aggregate, TVersion? oldVersion, int eventsSinceLastSnapshot)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            var dataSet = _implementation.Serialize(aggregate, oldVersion, eventsSinceLastSnapshot);

            try
            {
                // If the data-set contains a snapshot, the number or events
                // since the last snapshot is reset to 0, since this snapshot
                // represents the latest version of the aggregate.
                //
                // If not, then the remembered value is added to the new amount
                // of events that were published.
                if (dataSet.Snapshot == null)
                {
                    return dataSet.Events.Count + eventsSinceLastSnapshot;
                }
                return 0;
            }
            finally
            {
                changeSet.Add(dataSet);
            }            
        }        

        /// <summary>
        /// Creates and returns a strategy that uses only snapshots of an aggregate.
        /// Use this strategy if your data-store is state-based and/or you are using
        /// an ORM to store the snapshots.
        /// </summary>
        public static SerializationStrategy UseSnapshots() =>
            new SerializationStrategy(new UseSnapshotsImplementation());

        /// <summary>
        /// Creates and returns a strategy that stores all events that are published by the aggregate.
        /// </summary>
        /// <param name="eventsPerSnapshot">
        /// Determines if and when a new snapshot is taken of the aggregate when storing the events
        /// in order to optimize future read- and restore-operations.
        /// If this parameter is <c>0</c>, then no snapshots will be taken.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="eventsPerSnapshot"/> is negative.
        /// </exception>
        public static SerializationStrategy UseEvents(int eventsPerSnapshot = 0) =>
            new SerializationStrategy(new UseEventsImplementation(eventsPerSnapshot));                        
    }
}
