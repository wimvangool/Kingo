using System;

namespace Kingo.Messaging.Domain
{
    internal interface IDomainEventToPublish<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        void Publish(IDomainEventBus<TKey, TVersion> eventBus);

        EventToSave<TKey, TVersion> CreateEventToSave(ITypeToContractMap typeToContractMap);
    }
}
