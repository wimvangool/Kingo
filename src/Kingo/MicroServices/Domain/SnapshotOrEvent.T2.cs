using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base class for all snapshots and events that are related to a specific aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class SnapshotOrEvent<TKey, TVersion> : ISnapshotOrEvent<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        TKey ISnapshotOrEvent<TKey, TVersion>.Id =>
            Id;

        /// <summary>
        /// The identifier of the associated aggregate.
        /// </summary>
        protected abstract TKey Id
        {
            get;
        }

        TVersion ISnapshotOrEvent<TKey, TVersion>.Version =>
            Version;

        /// <summary>
        /// The version of the associated aggregate.
        /// </summary>
        protected abstract TVersion Version
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{ GetType().FriendlyName() } (Id = { Id }, Version = { Version })";

        ISnapshotOrEvent ISnapshotOrEvent.UpdateToNextVersion() =>
            UpdateToNextVersion();

        /// <summary>
        /// Updates this snapshot or event to the next version, if it exists.
        /// </summary>
        /// <returns>The next version of this snapshot or event, or <c>null</c> if this is the latest version.</returns>
        protected virtual ISnapshotOrEvent UpdateToNextVersion() =>
            null;

        IAggregateRoot<TKey, TVersion> ISnapshotOrEvent<TKey, TVersion>.RestoreAggregate(IEventBus eventBus) =>
            RestoreAggregate(eventBus);

        /// <summary>
        /// Restores and returns the aggregate this snapshot or event originated from.
        /// </summary>
        /// <param name="eventBus">The event bus to which all events published by the aggregate will be written to.</param>
        /// <returns>The restored aggregate.</returns>
        /// <exception cref="NotSupportedException">
        /// This instance represents an event that cannot be used to restore its associated aggregate.
        /// </exception>
        protected virtual IAggregateRoot<TKey, TVersion> RestoreAggregate(IEventBus eventBus = null) =>
            throw NewRestoreNotSupportedException();

        private Exception NewRestoreNotSupportedException()
        {
            var messageFormat = ExceptionMessages.SnapshotOrEvent_RestoreNotSupported;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new NotSupportedException(message);
        }
    }
}
