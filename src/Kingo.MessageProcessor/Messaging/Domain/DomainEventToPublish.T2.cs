using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.Messaging.Domain
{
    internal abstract class DomainEventToPublish<TKey, TVersion> : IDomainEventToPublish<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        #region [====== Instance Members ======]

        public abstract void Publish(IDomainEventBus<TKey, TVersion> eventBus);

        public abstract EventToSave<TKey, TVersion> CreateEventToSave(ITypeToContractMap typeToContractMap);

        #endregion

        #region [====== Static Members ======]

        private static readonly ConcurrentDictionary<Type, Func<IMessage, IDomainEventToPublish<TKey, TVersion>>> _FactoryMethods = new ConcurrentDictionary<Type, Func<IMessage, IDomainEventToPublish<TKey, TVersion>>>();

        public static IDomainEventToPublish<TKey, TVersion> FromMessage(IDomainEvent<TKey, TVersion> @event)            
        {
            return _FactoryMethods.GetOrAdd(@event.GetType(), CreateFactoryMethod).Invoke(@event);
        }

        private static Func<IMessage, IDomainEventToPublish<TKey, TVersion>> CreateFactoryMethod(Type eventType)
        {
            var eventParameter = Expression.Parameter(typeof(IMessage), "event");
            var @event = Expression.Convert(eventParameter, eventType);
            var expression = CreateConstructorInvocationExpression(@event);

            return Expression.Lambda<Func<IMessage, IDomainEventToPublish<TKey, TVersion>>>(expression, eventParameter).Compile();
        }

        private static Expression CreateConstructorInvocationExpression(Expression @event)
        {
            return Expression.New(FindConstructor(@event.Type), @event);
        }

        private static ConstructorInfo FindConstructor(Type eventType)
        {
            var eventToPublishTypeDefinition = typeof(DomainEventToPublish<,,>);
            var eventToPublishType = eventToPublishTypeDefinition.MakeGenericType(typeof(TKey), typeof(TVersion), eventType);

            return eventToPublishType.GetConstructor(new [] { eventType });
        }

        #endregion
    }
}
