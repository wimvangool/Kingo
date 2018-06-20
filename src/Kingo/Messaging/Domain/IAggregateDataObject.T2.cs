using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a piece of data that carries the id and version of an aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    public interface IAggregateDataObject<out TKey, out TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Identifier of an aggregate.
        /// </summary>
        TKey Id
        {
            get;
        }

        /// <summary>
        /// Version of an aggregate.
        /// </summary>
        TVersion Version
        {
            get;
        }
    }
}
