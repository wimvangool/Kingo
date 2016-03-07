namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents an event that is published by an application.
    /// </summary>
    public interface IDomainEvent : IMessage
    {
        /// <summary>
        /// Upgrades this event to the latest version.
        /// </summary>
        /// <returns>The latest version of this event.</returns>
        IDomainEvent UpgradeToLatestVersion(); 
    }
}
