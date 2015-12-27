using System;

namespace Kingo.Messaging.Domain
{
    internal sealed class DomainEventStream<TKey, TVersion> : IWritableEventStream<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>  
    {
        private readonly UnitOfWorkContext _context;

        internal DomainEventStream(UnitOfWorkContext context)
        {
            _context = context;
        }

        public void Write<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage<TEvent>
        {
            if (_context != null)
            {
                _context.Publish(@event);
            }
        }
    }
}
