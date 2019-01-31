using System;

namespace Kingo.MicroServices.Domain
{
    internal interface ISerializationStrategyFactory
    {
        SerializationStrategy<TKey, TVersion, TSnapshot, TAggregate> CreateSerializationStrategy<TKey, TVersion, TSnapshot, TAggregate>()
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
            where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>;
    }
}
