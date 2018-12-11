using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a snapshot of the state of an <see cref="IAggregateRoot" /> and serves as
    /// a factory to restore the aggregate in this state.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    public interface ISnapshot<TKey, TVersion> : ISnapshot, IAggregateDataObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Identifier of the aggregate.
        /// </summary>
        new TKey Id
        {
            get;
            set;
        }

        /// <summary>
        /// Version of the aggregate.
        /// </summary>
        new TVersion Version
        {
            get;
            set;
        }
    }
}
