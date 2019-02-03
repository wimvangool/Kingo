using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, contains a snapshot and a set of events that represent
    /// the state and state-changes of an aggregate, and which are meant to written to a data-store.    
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>    
    public interface IAggregateWriteSet<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        #region [====== Id & Version ======]

        /// <summary>
        /// Identifier of the aggregate.
        /// </summary>
        TKey Id
        {
            get;
        }

        /// <summary>
        /// The version of the aggregate that it had when it was read from the data-store. It
        /// has the default value for new (added) aggregates. 
        /// </summary>
        /// <remarks>
        /// This value can be used to throw a <see cref="ConcurrencyException" /> from the repository
        /// if this value does not match the value in the data-store, implying that the aggregate
        /// was updated in another transaction while the current transaction was running.
        /// </remarks>
        TVersion OldVersion
        {
            get;
        }

        /// <summary>
        /// The current version of the aggregate.
        /// </summary>
        TVersion NewVersion
        {
            get;
        }

        #endregion

        #region [====== Events ======]       

        /// <summary>
        /// Events that were published by the aggregate.
        /// </summary>
        IReadOnlyList<ISnapshotOrEvent<TKey, TVersion>> Events
        {
            get;
        }

        #endregion
    }
}
