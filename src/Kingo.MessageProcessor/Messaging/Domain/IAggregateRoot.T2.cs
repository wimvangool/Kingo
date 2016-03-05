using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents an aggregate root.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>     
    public interface IAggregateRoot<TKey, TVersion> : IHasKeyAndVersion<TKey, TVersion>, IReadableEventStream<TKey, TVersion>
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
        /// Creates and returns a <see cref="ISnapshot{T, S}" /> of this aggregate.
        /// </summary>
        /// <returns>A new snapshot of this aggregate.</returns>
        ISnapshot<TKey, TVersion> CreateSnapshot();
    }
}
