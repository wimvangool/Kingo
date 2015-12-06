using System;
using System.Runtime.Serialization;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events.
    /// </summary>
    /// <typeparam name="TMessage">Type of the implementing event.</typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TVersion"></typeparam>
    [Serializable]
    [DataContract]
    public abstract class DomainEvent<TMessage, TKey, TVersion> : Message<TMessage>, IVersionedObject<TKey, TVersion>
        where TMessage : DomainEvent<TMessage, TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent" /> class.
        /// </summary>
        protected DomainEvent() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected DomainEvent(DomainEvent<TMessage, TKey, TVersion> message)
            : base(message) { }

        #region [====== Key ======]

        TKey IKeyedObject<TKey>.Key
        {
            get { return GetKey(); }
        }

        /// <summary>
        /// Returns the key of the aggregate this event was published by.
        /// </summary>
        /// <returns>Key of the aggregate.</returns>
        protected virtual TKey GetKey()
        {
            return DomainEvent.GetKey<TKey>(this);
        }

        #endregion

        #region [====== Version ======]

        TVersion IVersionedObject<TKey, TVersion>.Version
        {
            get { return GetVersion(); }
        }

        /// <summary>
        /// Returns the version of the aggregate this event was published by.
        /// </summary>
        /// <returns>Version of the aggregate.</returns>
        protected virtual TVersion GetVersion()
        {
            return DomainEvent.GetVersion<TVersion>(this);
        }

        #endregion
    }
}
