using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    internal interface IBufferedEvent<out TKey> where TKey : struct, IEquatable<TKey>
    {
        void WriteTo(IWritableEventStream<TKey> stream);
    }
}
