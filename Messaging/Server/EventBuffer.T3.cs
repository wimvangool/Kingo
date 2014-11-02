namespace System.ComponentModel.Server
{
    internal sealed class EventBuffer<TKey, TVersion, TEvent> : IEventBuffer<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
        where TEvent : class, IAggregateEvent<TKey, TVersion>
    {
        private readonly TEvent _domainEvent;

        public EventBuffer(TEvent domainEvent)
        {            
            _domainEvent = domainEvent;
        }

        public void WriteTo(IWritableEventStream<TKey, TVersion> stream)
        {
            stream.Write(_domainEvent);
        }
    }
}
