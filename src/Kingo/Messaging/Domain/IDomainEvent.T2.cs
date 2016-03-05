using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a domain event that can be publised by the a certain aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    public interface IDomainEvent<TKey, TVersion> : IHasKeyAndVersion<TKey, TVersion>, IMessage
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
    }
}
