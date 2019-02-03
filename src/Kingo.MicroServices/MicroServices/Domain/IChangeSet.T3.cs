using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a set of changes that can be written and committed to a data store.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TSnapshot">Type of the snapshot of the aggregate.</typeparam>
    public interface IChangeSet<TKey, TVersion, out TSnapshot>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {
        /// <summary>
        /// A collection of aggregates that must be inserted into the data store.
        /// </summary>
        IReadOnlyList<IAggregateWriteSet<TKey, TVersion, TSnapshot>> AggregatesToInsert
        {
            get;
        }

        /// <summary>
        /// A collection of aggregates that must be updated in the data store.
        /// </summary>
        IReadOnlyList<IAggregateWriteSet<TKey, TVersion, TSnapshot>> AggregatesToUpdate
        {
            get;
        }

        /// <summary>
        /// A collection of identifiers representing the aggregates that must be deleted from the data store.
        /// </summary>
        IReadOnlyList<TKey> AggregatesToDelete
        {
            get;
        }
    }
}
