using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a factory that restores an <see cref="EventStream{T, S}" /> from a history of events
    /// and (optionally) one of its snapshots.
    /// </summary>
    /// <typeparam name="TKey">Key-type of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of the aggregate.</typeparam>    
    public sealed class EventStreamFactory<TKey, TVersion> : ISnapshot<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private readonly IHasVersion<TKey, TVersion>[] _orderedEvents;
        private readonly ISnapshot<TKey, TVersion> _snapshot;
        private readonly bool _useDefaultConstructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamFactory{T, S}" /> class.
        /// </summary>
        /// <param name="events">A collection of historic events.</param>
        /// <param name="useDefaultConstructor">
        /// Indicates whether or not this factory should invoke the default constructor of the aggregate to create the instance.
        /// Note that this constructor does not have to be public for the factory to find it. If <c>false</c>, this factory will create an
        /// instance of the aggregate without calling any constructor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        public EventStreamFactory(IEnumerable<IHasVersion<TKey, TVersion>> events, bool useDefaultConstructor = true)
            : this(events, null, useDefaultConstructor) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamFactory{T, S}" /> class.
        /// </summary>
        /// <param name="events">A collection of historic events.</param>
        /// <param name="snapshot">An (optional) snapshot of the aggregate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        public EventStreamFactory(IEnumerable<IHasVersion<TKey, TVersion>> events, ISnapshot<TKey, TVersion> snapshot)
            : this(events, snapshot, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamFactory{T, S}" /> class.
        /// </summary>
        /// <param name="events">A collection of historic events.</param>
        /// <param name="snapshot">An (optional) snapshot of the aggregate.</param>
        /// <param name="useDefaultConstructor">
        /// If <paramref name="snapshot"/> is <c>null</c>, indicates whether or not this factory should invoke the default constructor of
        /// the aggregate to create the instance. Note that this constructor does not have to be public for the factory
        /// to find it. If <c>false</c>, this factory will create an instance of the aggregate without calling any constructor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        public EventStreamFactory(IEnumerable<IHasVersion<TKey, TVersion>> events, ISnapshot<TKey, TVersion> snapshot, bool useDefaultConstructor)
        {
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }
            _orderedEvents = events.OrderBy(@event => @event.Version).ToArray();
            _snapshot = snapshot;
            _useDefaultConstructor = useDefaultConstructor;
        }

        TKey IHasKey<TKey>.Key
        {
            get
            {
                IHasVersion<TKey, TVersion> lastEvent;

                if (TryGetLastEvent(out lastEvent))
                {
                    return lastEvent.Key;
                }
                return default(TKey);
            }
        }

        TVersion IHasVersion<TKey, TVersion>.Version
        {
            get
            {
                IHasVersion<TKey, TVersion> lastEvent;

                if (TryGetLastEvent(out lastEvent))
                {
                    return lastEvent.Version;
                }
                return default(TVersion);
            }
        }
        
        private bool TryGetLastEvent(out IHasVersion<TKey, TVersion> lastEvent)
        {
            if (_orderedEvents.Length == 0)
            {
                lastEvent = null;
                return false;
            }
            lastEvent = _orderedEvents[_orderedEvents.Length - 1];
            return true;
        }

        /// <inheritdoc />
        public TAggregate RestoreAggregate<TAggregate>() where TAggregate : class, IAggregateRoot<TKey, TVersion>
        {            
            var aggregate = RestoreAggregate<TAggregate>(_snapshot);

            var writableStream = aggregate as IWritableEventStream<TKey, TVersion>;
            if (writableStream != null)
            {
                var readableStream = CreateMemoryEventStream(_orderedEvents);

                try
                {
                    readableStream.WriteTo(writableStream);
                }
                catch (ArgumentException exception)
                {
                    var messageFormat = ExceptionMessages.AggregateEventStreamFactory_InvalidEvents;
                    var message = string.Format(messageFormat, typeof(TAggregate));
                    throw new InvalidOperationException(message, exception);
                }
            }
            return aggregate;
        }

        private TAggregate RestoreAggregate<TAggregate>(ISnapshot<TKey, TVersion> snapshot) where TAggregate : class, IAggregateRoot<TKey, TVersion>
        {
            if (snapshot == null)
            {
                return _useDefaultConstructor
                    ? CreateAggregateWithConstructor<TAggregate>()
                    : CreateAggregateWithoutConstructor<TAggregate>();
            }
            return snapshot.RestoreAggregate<TAggregate>();
        }

        private static TAggregate CreateAggregateWithConstructor<TAggregate>()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var aggregateType = typeof(TAggregate);
            var constructor = aggregateType.GetConstructor(flags, null, new Type[0], null);
            if (constructor == null)
            {
                throw NewDefaultConstructorNotFoundException(aggregateType);
            }
            return (TAggregate) constructor.Invoke(new object[0]);
        }

        private static Exception NewDefaultConstructorNotFoundException(Type aggregateType)
        {
            var messageFormat = ExceptionMessages.AggregateEventStreamFactory_ConstructorNotFound;
            var message = string.Format(messageFormat, aggregateType);
            return new InvalidOperationException(message);
        }        

        private static TAggregate CreateAggregateWithoutConstructor<TAggregate>()
        {
            return (TAggregate) FormatterServices.GetUninitializedObject(typeof(TAggregate));
        }

        private static MemoryEventStream<TKey, TVersion> CreateMemoryEventStream(IEnumerable<IHasVersion<TKey, TVersion>> orderedEvents)
        {            
            var memoryEventStream = new MemoryEventStream<TKey, TVersion>();            

            foreach (var @event in orderedEvents)
            {
                memoryEventStream.Write(@event);
            }
            return memoryEventStream;
        }
    }
}
