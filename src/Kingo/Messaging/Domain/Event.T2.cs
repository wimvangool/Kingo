using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of events that were published by an aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class Event<TKey, TVersion> : AggregateDataObject<TKey, TVersion>, IEvent<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event{T, S}" /> class.
        /// </summary>
        /// <param name="id">Identifier of the aggregate.</param>
        /// <param name="version">Version of the aggregate.</param>
        protected Event(TKey id = default(TKey), TVersion version = default(TVersion)) :
            base(id, version) { }        

        IEvent IEvent.UpdateToLatestVersion() =>
            UpdateToLatestVersion();

        /// <summary>
        /// Converts this event to its latest version and returns the result. This method can be used to upgrade
        /// older versions of events that have been retrieved from an event store to a version that is compatible
        /// with the latest implementation of the <see cref="IAggregateRoot" />.        
        /// </summary>
        /// <returns>The latest version of this event.</returns>
        protected virtual IEvent UpdateToLatestVersion() =>
            this;

        internal override object RestoreAggregate(Type aggregateType) =>
            AggregateRootFactory.RestoreAggregateFromEvent(aggregateType, this);
    }
}
