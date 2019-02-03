using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents the root of an aggregate with a specific id.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of this aggregate.</typeparam>
    public interface IAggregateRoot<out TKey> : IAggregateRoot
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// The identifier of this aggregate.
        /// </summary>
        TKey Id
        {
            get;
        }
    }
}
