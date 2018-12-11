using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a set of changes that can be written and committed to a data store.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IChangeSet<TKey> where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// A collection of aggregates that must be inserted into the data store.
        /// </summary>
        IReadOnlyCollection<AggregateDataSet<TKey>> AggregatesToInsert
        {
            get;
        }

        /// <summary>
        /// A collection of aggregates that must be updated in the data store.
        /// </summary>
        IReadOnlyCollection<AggregateDataSet<TKey>> AggregatesToUpdate
        {
            get;
        }

        /// <summary>
        /// A collection of identifiers representing the aggregates that must be deleted from the data store.
        /// </summary>
        IReadOnlyCollection<TKey> AggregatesToDelete
        {
            get;
        }
    }
}
