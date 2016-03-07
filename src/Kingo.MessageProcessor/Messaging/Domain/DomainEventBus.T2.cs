using System;

namespace Kingo.Messaging.Domain
{
    internal sealed class DomainEventBus<TKey, TVersion> : IDomainEventBus<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly UnitOfWorkContext _context;

        internal DomainEventBus(UnitOfWorkContext context)
        {
            _context = context;
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : class, IDomainEvent<TKey, TVersion>
        {
            _context?.Publish(@event);            
        }
    }
}
