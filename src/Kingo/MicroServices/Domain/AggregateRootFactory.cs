using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.MicroServices.Domain
{
    internal sealed class AggregateRootFactory : IAggregateRootFactory
    {
        private readonly IAggregateRootFactory _snapshotOrCreatedEvent;
        private readonly IEnumerable<IEvent> _eventsToApply;

        private AggregateRootFactory(IAggregateRootFactory snapshotOrCreatedEvent, IEnumerable<IEvent> eventsToApply)
        {
            _snapshotOrCreatedEvent = snapshotOrCreatedEvent;
            _eventsToApply = eventsToApply;
        }

        public TAggregate RestoreAggregate<TAggregate>() where TAggregate : IAggregateRoot
        {
            if (_snapshotOrCreatedEvent == null)
            {
                throw NewMissingSnapshotOrCreatedEventException();
            }
            var aggregate = _snapshotOrCreatedEvent.RestoreAggregate<TAggregate>();
            aggregate.LoadFromHistory(_eventsToApply);
            return aggregate;
        }

        private static Exception NewMissingSnapshotOrCreatedEventException() =>
            new NotSupportedException(ExceptionMessages.AggregateRootFactory_MissingSnapshotOrCreatedEvent);

        public override string ToString() =>
            $"<{ToString(_snapshotOrCreatedEvent)}>{ToString(_eventsToApply)}";

        private static string ToString(object snapshotOrCreatedEvent) =>
            snapshotOrCreatedEvent == null ? "?" : snapshotOrCreatedEvent.GetType().FriendlyName();

        private static string ToString(IEnumerable<object> events) =>
            string.Concat(events.Select(@event => $"+{@event.GetType().FriendlyName()}"));

        public static AggregateRootFactory FromDataSet(ISnapshot snapshot, IReadOnlyList<IEvent> events) =>
            snapshot == null
                ? new AggregateRootFactory(events.FirstOrDefault(), events.Skip(1))
                : new AggregateRootFactory(snapshot, events);

        #region [====== Snapshot Constructor Invocation ======]

        private static readonly ConcurrentDictionary<Type, Func<object, object>> _SnapshotConstructors = new ConcurrentDictionary<Type, Func<object, object>>();

        internal static object RestoreAggregateFromSnapshot(Type aggregateType, object snapshot) =>
            _SnapshotConstructors.GetOrAdd(aggregateType, type => BuildSnapshotConstructorDelegate(type, snapshot.GetType())).Invoke(snapshot);

        private static Func<object, object> BuildSnapshotConstructorDelegate(Type aggregateType, Type snapshotType) =>
            BuildSnapshotConstructorInvocationExpression(aggregateType, snapshotType).Compile();

        private static Expression<Func<object, object>> BuildSnapshotConstructorInvocationExpression(Type aggregateType, Type snapshotType)
        {
            var snapshotParameter = Expression.Parameter(typeof(object), "snapshot");
            var snapshot = Expression.Convert(snapshotParameter, snapshotType);

            var constructor = FindConstructor(aggregateType, snapshotType);
            var constructorInvocation = Expression.New(constructor, snapshot);

            return Expression.Lambda<Func<object, object>>(constructorInvocation, snapshotParameter);
        }

        #endregion

        #region [====== Event Constructor Invocation ======]

        private static readonly ConcurrentDictionary<Type, Func<object, object>> _EventConstructors = new ConcurrentDictionary<Type, Func<object, object>>();

        internal static object RestoreAggregateFromEvent(Type aggregateType, object @event) =>
            _EventConstructors.GetOrAdd(aggregateType, type => BuildEventConstructorDelegate(type, @event.GetType())).Invoke(@event);

        private static Func<object, object> BuildEventConstructorDelegate(Type aggregateType, Type eventType) =>
            BuildEventConstructorInvocationExpression(aggregateType, eventType).Compile();

        private static Expression<Func<object, object>> BuildEventConstructorInvocationExpression(Type aggregateType, Type eventType)
        {
            var eventParameter = Expression.Parameter(typeof(object), "event");
            var @event = Expression.Convert(eventParameter, eventType);
            var isNewAggregate = Expression.Constant(false, typeof(bool));

            var constructor = FindConstructor(aggregateType, eventType, typeof(bool));
            var constructorInvocation = Expression.New(constructor, @event, isNewAggregate);

            return Expression.Lambda<Func<object, object>>(constructorInvocation, eventParameter);
        }

        #endregion

        private static ConstructorInfo FindConstructor(Type aggregateType, params Type[] parameterTypes)
        {
            if (IsNotInstantiableClassType(aggregateType))
            {
                throw NewNotInstantiableClassTypeException(aggregateType);
            }
            var constructor = aggregateType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
            if (constructor == null)
            {
                throw NewConstructorNotFoundException(aggregateType, parameterTypes);
            }
            return constructor;
        }        

        private static bool IsNotInstantiableClassType(Type aggregateType) =>
            !aggregateType.IsClass || aggregateType.IsAbstract;

        private static Exception NewNotInstantiableClassTypeException(Type aggregateType)
        {
            var messageFormat = ExceptionMessages.AggregateRootFactory_NotInstantiableClassType;
            var message = string.Format(messageFormat, aggregateType.FriendlyName());
            return new ArgumentException(message, nameof(aggregateType));
        }

        private static Exception NewConstructorNotFoundException(Type aggregateType, Type[] parameterTypes)
        {            
            var messageFormat = ExceptionMessages.AggregateRootFactory_ConstructorNotFound;
            var message = string.Format(messageFormat, aggregateType.FriendlyName(), ToString(parameterTypes));
            return new ArgumentException(message, nameof(aggregateType));
        }

        private static string ToString(IEnumerable<Type> types) =>
            string.Join(", ", types.Select(type => type.FriendlyName()));
    }
}
