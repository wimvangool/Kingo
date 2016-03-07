using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a snapshot of a certain type of aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>    
    public interface IMemento<TKey, TVersion> : IHasKeyAndVersion<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Restores an aggregate from this memento.
        /// </summary>
        /// <returns>The restored aggregate.</returns>
        /// <exception cref="InvalidOperationException">
        /// This memento could not restore the instance of the specified type.
        /// </exception>
        IAggregateRoot<TKey, TVersion> RestoreAggregate();
    }
}
