namespace System.ComponentModel.Messaging.Server
{
    public interface IBufferedEventStream<out TKey> where TKey : struct, IEquatable<TKey>
    {
        void FlushTo(IWritableEventStream<TKey> stream);
    }
}
