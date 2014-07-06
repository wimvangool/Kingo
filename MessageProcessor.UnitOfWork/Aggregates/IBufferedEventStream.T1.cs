using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public interface IBufferedEventStream<out TKey> where TKey : struct, IEquatable<TKey>
    {
        void FlushTo(IWritableEventStream<TKey> stream);
    }
}
