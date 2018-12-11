using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Represents an event that carries the id and version of an aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    public interface IEvent<TKey, TVersion> : IEvent, IAggregateDataObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Identifier of the aggregate.
        /// </summary>
        new TKey Id
        {
            get;
            set;
        }

        /// <summary>
        /// Version of the aggregate.
        /// </summary>
        new TVersion Version
        {
            get;
            set;
        }
    }
}
