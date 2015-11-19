using System;

namespace Kingo.BuildingBlocks.Messaging.Domain
{
    internal interface IEventBuffer<out TKey, out TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        void WriteTo(IWritableEventStream<TKey, TVersion> stream);
    }
}
