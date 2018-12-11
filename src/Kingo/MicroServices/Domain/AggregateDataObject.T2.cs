using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base class for all snapshots and events that are related to a specific aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class AggregateDataObject<TKey, TVersion> : IAggregateDataObject<TKey, TVersion>, IAggregateRootFactory
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private TKey _id;
        private TVersion _version;
        
        internal AggregateDataObject(TKey id = default(TKey), TVersion version = default(TVersion))
        {
            _id = id;
            _version = version;
        }        

        /// <summary>
        /// Identifier of the aggregate.
        /// </summary>
        public virtual TKey Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// Version of the aggregate.
        /// </summary>
        public virtual TVersion Version
        {
            get => _version;
            set => _version = value;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{ GetType().FriendlyName() } (Id = { Id }, Version = { Version })";

        TAggregate IAggregateRootFactory.RestoreAggregate<TAggregate>() =>
            RestoreAggregate<TAggregate>();

        /// <summary>
        /// Restores an aggregate and returns its root.
        /// </summary>      
        /// <exception cref="NotSupportedException">
        /// The operation is not supported by this instance.
        /// </exception> 
        protected virtual TAggregate RestoreAggregate<TAggregate>() =>
            (TAggregate) RestoreAggregate(typeof(TAggregate));

        internal abstract object RestoreAggregate(Type aggregateType);        
    }
}
