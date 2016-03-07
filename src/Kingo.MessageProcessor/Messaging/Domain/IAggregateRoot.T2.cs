using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents an aggregate root.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>     
    public interface IAggregateRoot<TKey, TVersion> : IHasKeyAndVersion<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to publish.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        void Publish<TEvent>(TEvent @event) where TEvent : class, IDomainEvent<TKey, TVersion>;

        /// <summary>
        /// Marks this aggregate as committed and publishes all latest events to the specified <paramref name="eventBus"/>.
        /// </summary> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventBus"/> is <c>null</c>.
        /// </exception>       
        void Commit(IDomainEventBus<TKey, TVersion> eventBus);

        /// <summary>
        /// Creates and returns a snap shot in the form of a <see cref="IMemento{TKey,TVersion}" /> of this aggregate.
        /// </summary>
        /// <returns>A new snapshot of this aggregate.</returns>
        IMemento<TKey, TVersion> CreateSnapshot();
    }
}
