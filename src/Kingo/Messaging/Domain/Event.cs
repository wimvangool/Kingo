using Kingo.Messaging.Validation;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IEvent"/> interface.
    /// </summary>
    public abstract class Event : Message, IEvent
    {
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

        /// <inheritdoc />
        public override string ToString() =>
            GetType().FriendlyName();
    }
}
