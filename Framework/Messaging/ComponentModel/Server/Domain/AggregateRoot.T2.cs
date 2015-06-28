using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate-version.</typeparam>
    [Serializable]
    public abstract class AggregateRoot<TKey, TVersion> : Entity<TKey>, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{K, V}" /> class.
        /// </summary>
        protected AggregateRoot() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey, TVersion}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AggregateRoot(SerializationInfo info, StreamingContext context) { }

        #region [====== Version ======]

        TVersion IVersionedObject<TKey, TVersion>.Version
        {
            get { return Version; }
        }        

        /// <summary>
        /// Gets or sets the version of this aggregate.
        /// </summary>
        protected abstract TVersion Version
        {
            get;
            set;
        }

        /// <summary>
        /// Creates and returns a new version as compared to the current version of the aggregate.
        /// </summary>
        /// <returns>A new version.</returns>
        protected abstract TVersion NewVersion();
        
        /// <summary>
        /// Compares the specified <paramref name="version"/> with the <paramref name="newVersion"/>
        /// and assigns the new version to <paramref name="version"/> if it is newer.
        /// </summary>
        /// <param name="version">The current version.</param>
        /// <param name="newVersion">The new version.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="newVersion"/> is smaller than or equal to <paramref name="version"/>.
        /// </exception>
        protected static void SetVersion(ref TVersion version, TVersion newVersion)
        {
            if (version.CompareTo(newVersion) < 0)
            {
                version = newVersion;
                return;
            }
            throw NewInvalidVersionException(newVersion, version);
        }

        #endregion                

        /// <summary>
        /// Publishes the event that is created using the specified <paramref name="eventFactory"/>.        
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that is created and written.</typeparam>
        /// <param name="eventFactory">The factory that is used to created the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is not being called inside a <see cref="UnitOfWorkScope" />.
        /// </exception>
        protected void Publish<TEvent>(Func<TKey, TVersion, TEvent> eventFactory) where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage<TEvent>
        {
            if (eventFactory == null)
            {
                throw new ArgumentNullException("eventFactory");
            }
            Publish(eventFactory.Invoke(Id, NewVersion()));
        }

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event that is published.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is not being called inside a <see cref="UnitOfWorkScope" />.
        /// </exception>
        internal virtual void Publish<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage<TEvent>
        {            
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            Apply(@event);

            MessageProcessor.Publish(@event);
        }

        internal virtual void Apply<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            if (@event.Key.Equals(Id))
            {
                try
                {
                    Version = @event.Version;
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw NewInvalidVersionException(@event, Version);
                }                                
                return;
            }
            throw NewInvalidKeyException(@event, Id);                                  
        }

        private static Exception NewInvalidKeyException<TEvent>(TEvent @event, TKey aggregateKey) where TEvent : class, IVersionedObject<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidKey;
            var message = string.Format(messageFormat, @event.Key, aggregateKey);
            return new ArgumentException(message, "event");
        }

        private static Exception NewInvalidVersionException(TVersion newVersion, TVersion oldVersion)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidVersion;
            var message = string.Format(messageFormat, newVersion, oldVersion);
            return new ArgumentException(message, "newVersion");
        } 

        private static Exception NewInvalidVersionException<TEvent>(TEvent @event, TVersion aggregateVersion) where TEvent : class, IVersionedObject<TKey, TVersion>
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidVersion;
            var message = string.Format(messageFormat, @event.Version, aggregateVersion);
            return new ArgumentException(message, "event");
        }        
    }
}
