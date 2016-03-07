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
    /// <typeparam name="TAggregate">Type of the aggregate.</typeparam>  
    public sealed class EventStreamHistory<TKey, TVersion, TAggregate> : IMemento<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IEventStream<TKey, TVersion>
    {        
        private readonly IDomainEvent<TKey, TVersion>[] _historicEvents;
        private readonly IMemento<TKey, TVersion> _snapshot;
        private readonly bool _useDefaultConstructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamHistory{TKey, TVersion, TAgggregate}" /> class.
        /// </summary>
        /// <param name="historicEvents">A collection of historic events.</param>
        /// <param name="useDefaultConstructor">
        /// Indicates whether or not this factory should invoke the default constructor of the aggregate to create the instance.
        /// Note that this constructor does not have to be public for the factory to find it. If <c>false</c>, this factory will create an
        /// instance of the aggregate without calling any constructor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="historicEvents"/> is <c>null</c>.
        /// </exception>
        public EventStreamHistory(IEnumerable<IDomainEvent<TKey, TVersion>> historicEvents, bool useDefaultConstructor = true)
            : this(historicEvents, null, useDefaultConstructor) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamHistory{TKey, TVersion, TAggregate}" /> class.
        /// </summary>
        /// <param name="historicEvents">A collection of historic events.</param>
        /// <param name="snapshot">An (optional) snapshot of the aggregate.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="historicEvents"/> is <c>null</c>.
        /// </exception>
        public EventStreamHistory(IEnumerable<IDomainEvent<TKey, TVersion>> historicEvents, IMemento<TKey, TVersion> snapshot)
            : this(historicEvents, snapshot, true) { }
        
        private EventStreamHistory(IEnumerable<IDomainEvent<TKey, TVersion>> historicEvents, IMemento<TKey, TVersion> snapshot, bool useDefaultConstructor)
        {
            if (historicEvents == null)
            {
                throw new ArgumentNullException(nameof(historicEvents));
            }
            _historicEvents = historicEvents.ToArray();
            _snapshot = snapshot;
            _useDefaultConstructor = useDefaultConstructor;
        }

        TKey IHasKey<TKey>.Key
        {
            get
            {
                IHasKeyAndVersion<TKey, TVersion> lastEvent;

                if (TryGetLastEvent(out lastEvent))
                {
                    return lastEvent.Key;
                }
                return default(TKey);
            }
        }

        TVersion IHasKeyAndVersion<TKey, TVersion>.Version
        {
            get
            {
                IHasKeyAndVersion<TKey, TVersion> lastEvent;

                if (TryGetLastEvent(out lastEvent))
                {
                    return lastEvent.Version;
                }
                return default(TVersion);
            }
        }
        
        private bool TryGetLastEvent(out IHasKeyAndVersion<TKey, TVersion> lastEvent)
        {
            if (_historicEvents.Length == 0)
            {
                lastEvent = null;
                return false;
            }
            lastEvent = _historicEvents[_historicEvents.Length - 1];
            return true;
        }

        IAggregateRoot<TKey, TVersion> IMemento<TKey, TVersion>.RestoreAggregate()
        {
            return RestoreAggregate();
        }

        /// <inheritdoc />
        public TAggregate RestoreAggregate()
        {            
            var aggregate = RestoreAggregate(_snapshot);

            try
            {
                aggregate.LoadFromHistory(_historicEvents);
            }
            catch (ArgumentException exception)
            {
                var messageFormat = ExceptionMessages.EventStreamHistory_InvalidEvents;
                var message = string.Format(messageFormat, typeof(TAggregate));
                throw new InvalidOperationException(message, exception);
            }
            return aggregate;
        }

        private TAggregate RestoreAggregate(IMemento<TKey, TVersion> memento)
        {
            if (memento == null)
            {
                return _useDefaultConstructor
                    ? CreateAggregateWithConstructor()
                    : CreateAggregateWithoutConstructor();
            }
            try
            {
                return (TAggregate) memento.RestoreAggregate();
            }
            catch (InvalidCastException)
            {
                throw NewInvalidMementoException(memento);
            }            
        }

        private static TAggregate CreateAggregateWithConstructor()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            var constructor = typeof(TAggregate).GetConstructor(flags, null, new Type[0], null);
            if (constructor == null)
            {
                throw NewDefaultConstructorNotFoundException(typeof(TAggregate));
            }
            return (TAggregate) constructor.Invoke(new object[0]);
        }               

        private static TAggregate CreateAggregateWithoutConstructor()
        {
            return (TAggregate) FormatterServices.GetUninitializedObject(typeof(TAggregate));
        }

        private static Exception NewInvalidMementoException(object memento)
        {
            var messageFormat = ExceptionMessages.EventStreamHistory_InvalidMemento;
            var message = string.Format(messageFormat, memento.GetType(), typeof(TAggregate));
            return new InvalidOperationException(message);
        }

        private static Exception NewDefaultConstructorNotFoundException(Type aggregateType)
        {
            var messageFormat = ExceptionMessages.EventStreamHistory_ConstructorNotFound;
            var message = string.Format(messageFormat, aggregateType);
            return new InvalidOperationException(message);
        }
    }
}
