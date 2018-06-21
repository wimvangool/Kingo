using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for all snapshots and events that are related to a specific aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class AggregateDataObject<TKey, TVersion> : IAggregateDataObject<TKey, TVersion>, IAggregateRootFactory, ISerializable
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
        /// Initializes a new instance of the <see cref="AggregateDataObject{T, S}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        internal AggregateDataObject(SerializationInfo info, StreamingContext context)
        {
            _id = (TKey)info.GetValue(nameof(Id), typeof(TKey));
            _version = (TVersion)info.GetValue(nameof(Version), typeof(TVersion));
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) =>
            GetObjectData(info, context);

        /// <summary>
        /// Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="StreamingContext" />) for this serialization.</param>        
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(Version), Version);
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
