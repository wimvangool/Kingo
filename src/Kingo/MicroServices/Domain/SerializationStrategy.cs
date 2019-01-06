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
                return new AggregateDataSet<TKey, TVersion>(snapshot, events, oldVersion);
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

            public abstract IAggregateRoot<TKey, TVersion> Deserialize<TKey, TVersion>(AggregateDataSet<TKey, TVersion> dataSet, IEventBus eventBus)
                where TKey : struct, IEquatable<TKey>
                where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>;
        }

        #endregion

        #region [====== UseSnapshotsImplementation ======]

        private sealed class UseSnapshotsImplementation : Implementation
        {
            public override string ToString() =>
                nameof(UseSnapshots);

            protected override IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> GetEvents<TKey, TVersion>(IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events) =>
                new ISnapshotOrEvent<TKey, TVersion>[0];

            public override IAggregateRoot<TKey, TVersion> Deserialize<TKey, TVersion>(AggregateDataSet<TKey, TVersion> dataSet, IEventBus eventBus)
            {
                if (dataSet.Snapshot == null)
                {
                    throw NewMissingSnapshotException(dataSet.ToDataSet());
                }
                return dataSet.Snapshot.RestoreAggregate(eventBus);
            }            
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

            private bool UseSnapshots =>
                0 < _eventsPerSnapshot;

            public override string ToString() =>
                $"{nameof(UseEvents)} (EventsPerSnapshot = {_eventsPerSnapshot})";

            protected override ISnapshotOrEvent<TKey, TVersion> GetSnapshot<TKey, TVersion>(IAggregateRoot<TKey, TVersion> aggregate, int eventsSinceLastSnapshot)
            {
                if (UseSnapshots && _eventsPerSnapshot <= eventsSinceLastSnapshot)
                {
                    return base.GetSnapshot(aggregate, eventsSinceLastSnapshot);
                }
                return null;
            }

            public override IAggregateRoot<TKey, TVersion> Deserialize<TKey, TVersion>(AggregateDataSet<TKey, TVersion> dataSet, IEventBus eventBus)
            {
                if (dataSet.Snapshot != null && UseSnapshots)
                {
                    return RestoreAggregate(eventBus, dataSet.Snapshot, dataSet.Events);
                }
                if (dataSet.Events.Count > 0)
                {
                    return RestoreAggregate(eventBus, dataSet.Events[0], dataSet.Events.Skip(1));
                }
                throw NewMissingSnapshotOrEventException(dataSet.ToDataSet());                
            }            

            private static IAggregateRoot<TKey, TVersion> RestoreAggregate<TKey, TVersion>(IEventBus eventBus, ISnapshotOrEvent<TKey, TVersion> factory, IEnumerable<ISnapshotOrEvent<TKey, TVersion>> events)
                where TKey : struct, IEquatable<TKey>
                where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            {
                var aggregate = factory.RestoreAggregate(eventBus);
                aggregate.LoadFromHistory(events);
                return aggregate;
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

        #region [====== Serialize ======]

        internal AggregateDataSet<TKey, TVersion> Serialize<TKey, TVersion>(IAggregateRoot<TKey, TVersion> aggregate, TVersion? oldVersion, int eventsSinceLastSnapshot)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            return _implementation.Serialize(aggregate, oldVersion, eventsSinceLastSnapshot);                                   
        }

        #endregion

        #region [====== Deserialize ======]

        internal TAggregate Deserialize<TKey, TVersion, TAggregate>(AggregateDataSet dataSet, IEventBus eventBus)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            where TAggregate : class, IAggregateRoot<TKey, TVersion>
        {
            try
            {
                return (TAggregate) _implementation.Deserialize(UpdateToLatestVersion<TKey, TVersion>(dataSet), eventBus);
            }            
            catch (Exception exception)
            {
                throw NewCouldNotRestoreAggregateException(dataSet, typeof(TAggregate), exception);
            }            
        }                 

        /// <summary>
        /// Updates this data-set to the latest version.
        /// </summary>
        /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
        /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
        /// <returns>The data-set that contains the latest versions of the snapshot and events.</returns>
        public AggregateDataSet<TKey, TVersion> UpdateToLatestVersion<TKey, TVersion>(AggregateDataSet dataSet)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            var snapshot = UpdateToLatestVersion<TKey, TVersion>(dataSet.Snapshot);
            var events = dataSet.Events.Select(UpdateToLatestVersion<TKey, TVersion>);
            return new AggregateDataSet<TKey, TVersion>(snapshot, events);
        }

        private static ISnapshotOrEvent<TKey, TVersion> UpdateToLatestVersion<TKey, TVersion>(ISnapshotOrEvent snapshotOrEvent)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            if (snapshotOrEvent == null)
            {
                return null;
            }
            ISnapshotOrEvent latestVersion;

            do
            {
                latestVersion = snapshotOrEvent;
            } while ((snapshotOrEvent = snapshotOrEvent.UpdateToNextVersion()) != null);

            return (ISnapshotOrEvent<TKey, TVersion>) latestVersion;
        }        

        private static Exception NewCouldNotRestoreAggregateException(AggregateDataSet dataSet, Type aggregateType, Exception innerException)
        {
            var messageFormat = ExceptionMessages.SerializationStrategy_CouldNotRestoreAggregate;
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), innerException.Message);
            return new CouldNotRestoreAggregateException(dataSet, message, innerException);
        }

        private static Exception NewMissingSnapshotException(AggregateDataSet dataSet) =>
            new CouldNotRestoreAggregateException(dataSet, ExceptionMessages.SerializationStrategy_MissingSnapshot);

        private static Exception NewMissingSnapshotOrEventException(AggregateDataSet dataSet) =>
            new CouldNotRestoreAggregateException(dataSet, ExceptionMessages.SerializationStrategy_MissingSnapshotOrEvent);

        #endregion

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
