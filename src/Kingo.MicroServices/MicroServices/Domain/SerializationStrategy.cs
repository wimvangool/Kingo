using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Represents the strategy a repository uses to serialize and deserialize its aggregates.
    /// </summary>    
    public sealed class SerializationStrategy : ISerializationStrategyFactory
    {        
        private readonly ISerializationStrategyFactory _implementation;

        private SerializationStrategy(ISerializationStrategyFactory implementation)
        {
            _implementation = implementation;
        }

        /// <inheritdoc />
        public override string ToString() =>
            _implementation.ToString();

        SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate> ISerializationStrategyFactory.CreateSerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>() =>
            _implementation.CreateSerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>();

        #region [====== UseSnapshots ======]

        private sealed class UseSnapshotStrategyFactory : ISerializationStrategyFactory
        {
            public override string ToString() =>
                nameof(UseSnapshots);

            SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate> ISerializationStrategyFactory.CreateSerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>() =>
                new UseSnapshotsStrategy<TKey, TVersion, TSnapshot, TAggregate>();
        }

        /// <summary>
        /// Creates and returns a strategy that uses only snapshots of an aggregate.
        /// Use this strategy if your data-store is state-based and/or you are using
        /// an ORM to store the snapshots.
        /// </summary>
        public static SerializationStrategy UseSnapshots() =>
            new SerializationStrategy(new UseSnapshotStrategyFactory());

        #endregion

        #region [====== UseEvents ======]

        private sealed class UseEventsStrategyFactory : ISerializationStrategyFactory
        {
            private readonly int _eventsPerSnapshot;

            public UseEventsStrategyFactory(int eventPerSnapshot)
            {
                if (eventPerSnapshot < 0)
                {
                    throw NewInvalidEventsPerSnapshotException(eventPerSnapshot);
                }
                _eventsPerSnapshot = eventPerSnapshot;
            }

            public override string ToString() =>
                $"{nameof(UseEvents)} ({ToString(_eventsPerSnapshot)})";

            private static string ToString(int eventsPerSnapshot) =>
                eventsPerSnapshot == 0 ? "No snapshots" : $"One snapshot per {eventsPerSnapshot} event(s)";

            SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate> ISerializationStrategyFactory.CreateSerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>() =>
                new UseEventsStrategy<TKey, TVersion, TSnapshot, TAggregate>(_eventsPerSnapshot);

            private static Exception NewInvalidEventsPerSnapshotException(int eventsPerSnapshot)
            {
                var messageFormat = ExceptionMessages.SerializationStrategy_InvalidEventsPerSnapshot;
                var message = string.Format(messageFormat, eventsPerSnapshot);
                return new ArgumentOutOfRangeException(nameof(eventsPerSnapshot), message);
            }
        }

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
            new SerializationStrategy(new UseEventsStrategyFactory(eventsPerSnapshot));

        #endregion
    }
}
