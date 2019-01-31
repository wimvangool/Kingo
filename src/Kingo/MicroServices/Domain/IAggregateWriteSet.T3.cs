using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, contains a snapshot and a set of events that represent
    /// the state and state-changes of an aggregate, and which are meant to written to a data-store.    
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TSnapshot">Type of the snapshot of the aggregate.</typeparam>
    public interface IAggregateWriteSet<TKey, TVersion, out TSnapshot> : IAggregateWriteSet<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {        
        #region [====== Snapshot ======]

        /// <summary>
        /// Snapshot that was taken from the aggregate (or <c>null</c> if none was taken).
        /// </summary>
        TSnapshot Snapshot
        {
            get;
        }        

        #endregion
    }
}
