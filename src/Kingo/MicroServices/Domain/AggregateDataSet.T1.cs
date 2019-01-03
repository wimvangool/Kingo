using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Contains a snapshot and a set of events that represent the state and state-changes of an aggregate.
    /// </summary>    
    public class AggregateDataSet<TKey>
        where TKey : struct, IEquatable<TKey>
    {             
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDataSet{TKey}" /> class.
        /// </summary>
        /// <param name="id">Unique identifier of the aggregate.</param>
        /// <param name="snapshot">Snapshot of an aggregate.</param>
        /// <param name="events">A collection of events published by an aggregate.</param>        
        public AggregateDataSet(TKey id, ISnapshotOrEvent snapshot, IEnumerable<ISnapshotOrEvent> events = null)
        {
            Id = id;
            Snapshot = snapshot;
            Events = (events ?? Enumerable.Empty<ISnapshotOrEvent>()).ToArray();
        }        

        /// <summary>
        /// Identifier of the aggregate,
        /// </summary>
        public TKey Id
        {
            get;
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

        #region [====== UpdateToLatestVersion ======]

        /// <summary>
        /// Updates this data-set to the latest version.
        /// </summary>
        /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
        /// <returns>The data-set that contains the latest versions of the snapshot and events.</returns>
        public AggregateDataSet<TKey, TVersion> UpdateToLatestVersion<TVersion>()
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            var snapshot = UpdateToLatestVersion<TVersion>(Snapshot);
            var events = Events.Select(UpdateToLatestVersion<TVersion>);
            return new AggregateDataSet<TKey, TVersion>(Id, snapshot, events);
        }

        private static ISnapshotOrEvent<TKey, TVersion> UpdateToLatestVersion<TVersion>(ISnapshotOrEvent snapshotOrEvent)
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
            
            return Convert<TVersion>(latestVersion);
        }

        private static ISnapshotOrEvent<TKey, TVersion> Convert<TVersion>(ISnapshotOrEvent snapshotOrEvent)
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            try
            {
                return (ISnapshotOrEvent<TKey, TVersion>) snapshotOrEvent;
            }
            catch (InvalidCastException)
            {
                throw NewUnexpectedSnapshotOrEventTypeException(snapshotOrEvent, typeof(ISnapshotOrEvent<TKey, TVersion>));
            }
        }

        private static Exception NewUnexpectedSnapshotOrEventTypeException(ISnapshotOrEvent snapshotOrEvent, Type interfaceType)
        {
            var messageFormat = ExceptionMessages.AggregateDataSet_UnexpectedSnapshotOrEventType;
            var message = string.Format(messageFormat, snapshotOrEvent.GetType().FriendlyName(), interfaceType.FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
