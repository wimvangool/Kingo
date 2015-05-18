namespace System.ComponentModel.Server.Domain
{
    internal sealed class EventBuffer<TKey, TVersion, TEvent> : IEventBuffer<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TEvent : class, IVersionedObject<TKey, TVersion>
    {
        private readonly TEvent _domainEvent;

        internal EventBuffer(TEvent domainEvent)
        {            
            _domainEvent = domainEvent;
        }

        public void WriteTo(IWritableEventStream<TKey, TVersion> stream)
        {
            stream.Write(_domainEvent);
        }
    }
}
