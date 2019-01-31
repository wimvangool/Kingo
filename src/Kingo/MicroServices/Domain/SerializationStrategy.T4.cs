using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    internal abstract class SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
    {
        #region [====== Serialize ======]        

        public AggregateWriteSet<TKey, TVersion, TSnapshot> Serialize(IAggregateRoot<TKey, TVersion, TSnapshot> aggregate, TVersion? oldVersion, int eventsSinceLastSnapshot)            
        {            
            var events = GetEvents(aggregate.Commit());
            var snapshot = GetSnapshot(aggregate, events.Count + eventsSinceLastSnapshot);
            return new AggregateWriteSet<TKey, TVersion, TSnapshot>(snapshot, events, oldVersion);
        }

        protected virtual IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> GetEvents(IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> events) =>
            events;

        protected virtual TSnapshot GetSnapshot(IAggregateRoot<TKey, TVersion, TSnapshot> aggregate, int eventsSinceLastSnapshot) =>
            aggregate.TakeSnapshot();

        #endregion

        #region [====== Deserialize ======]

        public TAggregate Deserialize(AggregateReadSet dataSet, IEventBus eventBus)            
        {
            try
            {
                return (TAggregate) Deserialize(UpdateToLatestVersion(dataSet), eventBus);
            }
            catch (Exception exception)
            {
                throw NewCouldNotRestoreAggregateException(dataSet, typeof(TAggregate), exception);
            }
        }

        protected abstract IAggregateRoot<TKey, TVersion> Deserialize(AggregateWriteSet<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>> dataSet, IEventBus eventBus);           

        /// <summary>
        /// Updates this data-set to the latest version.
        /// </summary>
        /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
        /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
        /// <returns>The data-set that contains the latest versions of the snapshot and events.</returns>
        public AggregateWriteSet<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>> UpdateToLatestVersion(AggregateReadSet dataSet)            
        {
            var snapshot = UpdateToLatestVersion(dataSet.Snapshot);
            var events = dataSet.Events.Select(UpdateToLatestVersion);
            return new AggregateWriteSet<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>>(snapshot, events);
        }

        private static ISnapshotOrEvent<TKey, TVersion> UpdateToLatestVersion(ISnapshotOrEvent snapshotOrEvent)            
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

        private static Exception NewCouldNotRestoreAggregateException(AggregateReadSet dataSet, Type aggregateType, Exception innerException)
        {
            var messageFormat = ExceptionMessages.SerializationStrategy_CouldNotRestoreAggregate;
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), innerException.Message);
            return new CouldNotRestoreAggregateException(dataSet, message, innerException);
        }

        #endregion
    }
}
