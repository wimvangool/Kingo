namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represent an event message that may be published or stored in an event store.
    /// </summary>
    public interface IEvent : IAggregateRootFactory
    {
        /// <summary>
        /// Converts this event to its latest version and returns the result. This method can be used to upgrade
        /// older versions of events that have been retrieved from an event store to a version that is compatible
        /// with the latest implementation of the <see cref="IAggregateRoot" />.        
        /// </summary>
        /// <returns>The latest version of this event.</returns>
        IEvent UpdateToLatestVersion();
    }
}
