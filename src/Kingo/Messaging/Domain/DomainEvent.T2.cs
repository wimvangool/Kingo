using System;
using System.Runtime.Serialization;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events.
    /// </summary>
    /// <typeparam name="TKey">Key-type of the associated aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of the associated aggregate.</typeparam>    
    [Serializable]
    [DataContract]
    public abstract class DomainEvent<TKey, TVersion> : Message, IVersionedObject<TKey, TVersion>        
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent" /> class.
        /// </summary>
        /// <param name="message">The message to copy.</param>        
        protected DomainEvent(DomainEvent<TKey, TVersion> message = null)
            : base(message) { }

        #region [====== Key ======]

        TKey IKeyedObject<TKey>.Key
        {
            get { return Key; }
        }

        /// <summary>
        /// Returns the key of the aggregate this event was published by.
        /// </summary>
        /// <returns>Key of the aggregate.</returns>
        protected virtual TKey Key
        {
            get { return DomainEvent.GetKey<TKey>(this); }
        }

        #endregion

        #region [====== Version ======]

        TVersion IVersionedObject<TKey, TVersion>.Version
        {
            get { return Version; }
        }

        /// <summary>
        /// Returns the version of the aggregate this event was published by.
        /// </summary>
        /// <returns>Version of the aggregate.</returns>
        protected virtual TVersion Version
        {
            get { return DomainEvent.GetVersion<TVersion>(this); }
        }

        #endregion        
    }
}
