using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an internal message-bus that can be used to publish domain events and let all subscribers,
    /// direct or indirect, handle these events within the same session/transaction they were published in.
    /// </summary> 
    public interface IDomainEventBus<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Published the specified <paramref name="event"/> on this bus.
        /// </summary>        
        /// <param name="event">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        void Publish<TEvent>(TEvent @event)            
            where TEvent : class, IDomainEvent<TKey, TVersion>;
    }
}
