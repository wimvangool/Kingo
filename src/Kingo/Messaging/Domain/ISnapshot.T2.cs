using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents a snapshot of the state of an <see cref="IAggregateRoot" /> and serves as
    /// a factory to restore the aggregate in this state.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    public interface ISnapshot<out TKey, out TVersion> : ISnapshot
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// The identifier of the aggregate that created this snapshot.
        /// </summary>
        TKey Id
        {
            get;            
        }

        /// <summary>
        /// The version of the aggregate at the time it created this snapshot.
        /// </summary>
        TVersion Version
        {
            get;            
        }
    }
}
