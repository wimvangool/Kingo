using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents an aggregate that can be re-initialized by a collection of
    /// historic events.
    /// </summary>
    /// <typeparam name="TKey">Key-type of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of the aggregate.</typeparam>
    public interface IEventStream<TKey, TVersion> : IAggregateRoot<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Re-initializes this event by re-appyling all specified events in ascending order.
        /// </summary>
        /// <param name="historicEvents">A collection of historic events.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="historicEvents"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="historicEvents"/> contains an event that is not supported by this aggregate.
        /// </exception>
        void LoadFromHistory(IEnumerable<IDomainEvent<TKey, TVersion>> historicEvents);
    }
}
