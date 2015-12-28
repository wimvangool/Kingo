using System;

namespace Kingo.Messaging.Domain
{
    internal interface IEventBuffer<out TKey, out TVersion>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        void WriteTo(IWritableEventStream<TKey, TVersion> stream);
    }
}
