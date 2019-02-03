using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents the root of an aggregate with a specific id, version and snapshot.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of this aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of this aggregate.</typeparam>
    /// <typeparam name="TSnapshot">Type of the snapshot of this aggregate.</typeparam>
    public interface IAggregateRoot<TKey, TVersion, out TSnapshot> : IAggregateRoot<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
    {        
        #region [====== TakeSnapshot ======]               

        /// <summary>
        /// Creates and returns a snapshot of the current state of this aggregate.
        /// </summary>
        /// <returns>A snapshot of the current state of this aggregate.</returns>
        /// <exception cref="NotSupportedException">
        /// This aggregate does not support snapshots.
        /// </exception>
        TSnapshot TakeSnapshot();        

        #endregion
    }
}
