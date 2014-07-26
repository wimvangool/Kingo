namespace System.ComponentModel.Messaging.Server
{
    internal sealed class BufferedEvent<TKey, TVersion, TEvent> : IBufferedEvent<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
        where TEvent : class, IAggregateEvent<TKey, TVersion>
    {
        private readonly TEvent _domainEvent;

        public BufferedEvent(TEvent domainEvent)
        {            
            _domainEvent = domainEvent;
        }

        public void WriteTo(IWritableEventStream<TKey, TVersion> stream)
        {
            stream.Write(_domainEvent);
        }
    }
}
