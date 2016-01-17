using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a <see cref="Repository{T, K, S}" /> that stores its aggregates as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class AggregateEventStreamRepository<TKey, TVersion, TAggregate> : Repository<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>, IWritableEventStream<TKey, TVersion>
    {
        /// <summary>
        /// Returns the map that maps each type to a specific contract, which is used to serialize and deserialize snapshots and events.
        /// </summary>
        protected abstract ITypeToContractMap TypeToContractMap
        {
            get;
        }

        #region [====== Select ======]

        internal override async Task<TAggregate> SelectByKeyAndRestoreAsync(TKey key)
        {
            var factory = await SelectByKeyAsync(key, TypeToContractMap);
            if (factory == null)
            {
                return null;
            }
            return factory.RestoreAggregate<TAggregate>();
        }

        /// <summary>
        /// Loads all historic events and/or a snapshot of an aggregate and returns a factory by which the aggregate can be restored.
        /// </summary>
        /// <param name="key">Key of the aggregate.</param>
        /// <param name="typeToContractMap">
        /// The mapping from each contract to a specific type that can be used to deserialize the retrieved data to its correct type.
        /// </param>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the load operation. The task should return <c>null</c> if the aggregate was not found.
        /// </returns>
        protected abstract Task<AggregateEventStreamFactory<TKey, TVersion>> SelectByKeyAsync(TKey key, ITypeToContractMap typeToContractMap);

        #endregion

        #region [====== Insert ======]

        private sealed class EventBuffer : IWritableEventStream<TKey, TVersion>
        {            
            private readonly IWritableEventStream<TKey, TVersion> _domainEventStream;
            private readonly MemoryEventStream<TKey, TVersion> _memoryEventStream;

            private EventBuffer(IWritableEventStream<TKey, TVersion> domainEventStream)
            {
                _memoryEventStream = new MemoryEventStream<TKey, TVersion>();
                _domainEventStream = domainEventStream;
            }

            void IWritableEventStream<TKey, TVersion>.Write<TEvent>(TEvent @event)
            {
                _memoryEventStream.Write(@event);
            }            

            internal IEnumerable<Event<TKey, TVersion>> GetEvents(ITypeToContractMap typeToContractMap)
            {
                return from @event in _memoryEventStream.ToList()
                       select new Event<TKey, TVersion>(typeToContractMap, @event);
            }

            internal void Flush()
            {
                _memoryEventStream.WriteTo(_domainEventStream);
            }

            internal static EventBuffer FromAggregate(TAggregate aggregate, IWritableEventStream<TKey, TVersion> domainEventStream)
            {
                var eventBuffer = new EventBuffer(domainEventStream);
                aggregate.WriteTo(eventBuffer);
                return eventBuffer;
            }
        }      

        internal override async Task InsertAsync(TAggregate aggregate, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            var eventBuffer = EventBuffer.FromAggregate(aggregate, domainEventStream);
            var snapshot = new Snapshot<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot());
            var events = eventBuffer.GetEvents(TypeToContractMap);

            await InsertEventsAsync(snapshot, null, events);

            eventBuffer.Flush();
        }

        internal override async Task<bool> UpdateAsync(TAggregate aggregate, TVersion originalVersion, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            var eventBuffer = EventBuffer.FromAggregate(aggregate, domainEventStream);
            var snapshot = new Snapshot<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot());
            var events = eventBuffer.GetEvents(TypeToContractMap);

            var updateSucceeded = await InsertEventsAsync(snapshot, originalVersion, events);
            if (updateSucceeded)
            {
                eventBuffer.Flush();
                return true;
            }
            return false;
        }

        /// <summary>
        /// When implemented, appends all specified <paramref name="events"/> to the event store. As an optimization technique,
        /// this method can also be used to store a snapshot of the specified <paramref name="snapshot" />, so that the number of
        /// events required to read from the event store can be maximized.
        /// </summary>
        /// <param name="snapshot">The aggregate that was created or updated.</param>
        /// <param name="originalVersion">The original version of the aggregate. Will be <c>null</c> if the aggregate is new.</param>
        /// <param name="events">A collection of events that were published during this session and need to be stored in the event store.</param>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the insert operation. This task should return
        /// <c>true</c> if the flush succeeded or <c>false</c> if a concurrency conflict was detected.
        /// </returns>
        protected abstract Task<bool> InsertEventsAsync(Snapshot<TKey, TVersion> snapshot, TVersion? originalVersion, IEnumerable<Event<TKey, TVersion>> events);

        #endregion        
    }
}
