using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a repository that stores its aggregates as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class EventStore<TKey, TVersion, TAggregate> : Repository<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IEventStream<TKey, TVersion>
    {
        #region [====== Select ======]

        internal override async Task<TAggregate> SelectByKeyAsync(TKey key)
        {
            var history = await SelectHistoryByKeyAsync(key, TypeToContractMap);
            if (history == null)
            {
                return null;
            }
            return history.RestoreAggregate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="typeToContractMap"></param>
        /// <returns></returns>
        protected abstract Task<EventStreamHistory<TKey, TVersion, TAggregate>> SelectHistoryByKeyAsync(TKey key, ITypeToContractMap typeToContractMap);

        #endregion

        #region [====== Insert ======]

        private sealed class EventBuffer : IDomainEventBus<TKey, TVersion>
        {
            private readonly IDomainEventBus<TKey, TVersion> _eventBus;
            private readonly List<IDomainEventToPublish<TKey, TVersion>> _eventsToPublish;            

            private EventBuffer(IDomainEventBus<TKey, TVersion> eventBus)
            {
                _eventsToPublish = new List<IDomainEventToPublish<TKey, TVersion>>();
                _eventBus = eventBus;
            }

            void IDomainEventBus<TKey, TVersion>.Publish<TEvent>(TEvent @event)
            {
                _eventsToPublish.Add(new DomainEventToPublish<TKey, TVersion, TEvent>(@event));
            }

            internal IEnumerable<EventToSave<TKey, TVersion>> GetEventsToSave(ITypeToContractMap typeToContractMap)
            {
                return from @event in _eventsToPublish
                       select @event.CreateEventToSave(typeToContractMap);
            }

            internal void Flush()
            {
                foreach (var eventToPublish in _eventsToPublish)
                {
                    eventToPublish.Publish(_eventBus);
                }
            }

            internal static EventBuffer FromAggregate(TAggregate aggregate, IDomainEventBus<TKey, TVersion> eventBus)
            {
                var eventBuffer = new EventBuffer(eventBus);
                aggregate.Commit(eventBuffer);
                return eventBuffer;
            }
        }

        internal override async Task InsertAsync(TAggregate aggregate, IDomainEventBus<TKey, TVersion> eventBus)
        {
            var eventBuffer = EventBuffer.FromAggregate(aggregate, eventBus);
            var snapshot = new SnapshotToSave<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot());
            var events = eventBuffer.GetEventsToSave(TypeToContractMap);

            await InsertEventsAsync(snapshot, null, events);

            eventBuffer.Flush();
        }

        internal override async Task<bool> UpdateAsync(TAggregate aggregate, TVersion originalVersion, IDomainEventBus<TKey, TVersion> eventBus)
        {
            var eventBuffer = EventBuffer.FromAggregate(aggregate, eventBus);
            var snapshot = new SnapshotToSave<TKey, TVersion>(TypeToContractMap, aggregate.CreateSnapshot());
            var events = eventBuffer.GetEventsToSave(TypeToContractMap);

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
        /// this method can also be used to store a snapshot of the specified <paramref name="snapshotToSave" />, so that the number of
        /// events required to read from the event store can be maximized.
        /// </summary>
        /// <param name="snapshotToSave">The aggregate that was created or updated.</param>
        /// <param name="originalVersion">The original version of the aggregate. Will be <c>null</c> if the aggregate is new.</param>
        /// <param name="events">A collection of events that were published during this session and need to be stored in the event store.</param>
        /// <returns>
        /// A <see cref="Task{T}" /> representing the insert operation. This task should return
        /// <c>true</c> if the flush succeeded or <c>false</c> if a concurrency conflict was detected.
        /// </returns>
        protected abstract Task<bool> InsertEventsAsync(SnapshotToSave<TKey, TVersion> snapshotToSave, TVersion? originalVersion, IEnumerable<EventToSave<TKey, TVersion>> events);

        #endregion        
    }
}
