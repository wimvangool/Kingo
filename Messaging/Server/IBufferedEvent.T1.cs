namespace System.ComponentModel.Messaging.Server
{
    internal interface IBufferedEvent<out TKey> where TKey : struct, IEquatable<TKey>
    {
        void WriteTo(IWritableEventStream<TKey> stream);
    }
}
