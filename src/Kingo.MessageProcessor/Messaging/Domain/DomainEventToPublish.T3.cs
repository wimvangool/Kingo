using System;

namespace Kingo.Messaging.Domain
{
    internal sealed class DomainEventToPublish<TKey, TVersion, TEvent> : DomainEventToPublish<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TEvent : class, IDomainEvent<TKey, TVersion>
    {
        private readonly TEvent _event;

        public DomainEventToPublish(TEvent @event)
        {            
            _event = @event;
        }

        public override void Publish(IDomainEventBus<TKey, TVersion> eventBus)
        {
            eventBus.Publish(_event);
        }

        public override EventToSave<TKey, TVersion> CreateEventToSave(ITypeToContractMap typeToContractMap)
        {
            return new EventToSave<TKey, TVersion>(typeToContractMap, _event);
        }
    }
}
