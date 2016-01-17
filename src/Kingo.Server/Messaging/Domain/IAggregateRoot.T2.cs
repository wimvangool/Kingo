using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents an aggregate root.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>     
    public interface IAggregateRoot<TKey, TVersion> : IVersionedObject<TKey, TVersion>, IReadableEventStream<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        /// <summary>
        /// Creates and returns a <see cref="ISnapshot{T, S}" /> of this aggregate.
        /// </summary>
        /// <returns>A new snapshot of this aggregate.</returns>
        ISnapshot<TKey, TVersion> CreateSnapshot();
    }
}
