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
