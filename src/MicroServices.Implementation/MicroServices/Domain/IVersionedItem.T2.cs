using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents an item that has a unique identifier and version.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier.</typeparam>
    /// <typeparam name="TVersion">Type of the version.</typeparam>
    public interface IVersionedItem<out TKey, out TVersion> : IKeyedItem<TKey>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// The version of this item.
        /// </summary>
        TVersion Version
        {
            get;
        }
    }
}
