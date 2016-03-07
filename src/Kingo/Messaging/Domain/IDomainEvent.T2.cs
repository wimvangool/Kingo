using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a domain event that can be publised by the a certain aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    public interface IDomainEvent<TKey, TVersion> : IDomainEvent, IHasKeyAndVersion<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Gets or sets the key of the aggregate.
        /// </summary>
        new TKey Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the aggregate.
        /// </summary>
        new TVersion Version
        {
            get;
            set;
        }

        /// <summary>
        /// Upgrades this event to the latest version.
        /// </summary>
        /// <returns>The latest version of this event.</returns>
        new IDomainEvent<TKey, TVersion> UpgradeToLatestVersion();
    }
}
