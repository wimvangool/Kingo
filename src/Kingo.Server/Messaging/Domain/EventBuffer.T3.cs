using System;

namespace Kingo.Messaging.Domain
{
    internal sealed class EventBuffer<TKey, TVersion, TEvent> : IEventBuffer<TKey, TVersion>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage
    {
        private readonly TEvent _event;

        internal EventBuffer(TEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            _event = @event;
        }

        public void WriteTo(IWritableEventStream<TKey, TVersion> stream)
        {
            stream.Write(_event);
        }
    }
}
