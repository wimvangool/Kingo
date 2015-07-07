using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Syztem.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an entity that can be identified by it's key.
    /// </summary>
    /// <typeparam name="TKey">Type of the key of this entity.</typeparam>
    [Serializable]
    public abstract class Entity<TKey> : IKeyedObject<TKey>, ISerializable
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{T}" /> class.
        /// </summary>
        protected Entity() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{T}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected Entity(SerializationInfo info, StreamingContext context) { }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]        
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context) { }

        #region [====== Key ======]

        TKey IKeyedObject<TKey>.Key
        {
            get { return Id; }
        }

        /// <summary>
        /// Identifier of this entity.
        /// </summary>
        public abstract TKey Id
        {
            get;
        }

        #endregion        
    }
}
