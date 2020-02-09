using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a set of items that
    /// have been inserted, updated and deleted as part of a unit of work.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier.</typeparam>
    /// <typeparam name="TVersion">Type of the version number or timestamp.</typeparam>
    /// <typeparam name="TItem">Type of the items managed by this repository.</typeparam>
    public interface IChangeSet<TKey, TVersion, TItem>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TItem : class, IVersionedItem<TKey, TVersion>
    {

    }
}
