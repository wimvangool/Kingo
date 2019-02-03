namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents either a snapshot of an aggregate, or an event published by an aggregate.
    /// </summary>
    public interface ISnapshotOrEvent
    {
        /// <summary>
        /// Updates this snapshot or event to the next version, if it exists.
        /// </summary>
        /// <returns>The next version of this snapshot or event, or <c>null</c> if this is the latest version.</returns>
        ISnapshotOrEvent UpdateToNextVersion();        
    }
}
