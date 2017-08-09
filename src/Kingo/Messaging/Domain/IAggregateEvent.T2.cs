using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represent an event that was published by an aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    public interface IAggregateEvent<TKey, TVersion> : IEvent
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// The identifier of the aggregate that published this event.
        /// </summary>
        TKey AggregateId
        {
            get;
            set;
        }

        /// <summary>
        /// The version of the aggregate at the time it published this event.
        /// </summary>
        TVersion AggregateVersion
        {
            get;
            set;
        }
    }
}
