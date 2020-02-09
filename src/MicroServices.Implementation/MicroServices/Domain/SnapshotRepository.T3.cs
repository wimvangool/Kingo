using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented, represents a repository that manages snapshot-data of an entity.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier.</typeparam>
    /// <typeparam name="TVersion">Type of the version.</typeparam>
    /// <typeparam name="TSnapshot">Type of the snapshot.</typeparam>
    public abstract class SnapshotRepository<TKey, TVersion, TSnapshot> : Repository<TKey, TVersion, TSnapshot>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, IVersionedItem<TKey, TVersion>
    {
        

        

        #region [====== Write Methods ======]

        #endregion
    }
}
