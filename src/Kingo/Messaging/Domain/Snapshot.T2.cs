using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="ISnapshot{T, S}"/> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    [Serializable]
    public abstract class Snapshot<TKey, TVersion> : Snapshot, ISnapshot<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        TKey ISnapshot<TKey, TVersion>.Id =>
            Id;

        /// <summary>
        /// Gets or sets the identifier of the associated aggregate.
        /// </summary>
        public abstract TKey Id
        {
            get;
            set;
        }

        TVersion ISnapshot<TKey, TVersion>.Version =>

            Version;

        /// <summary>
        /// Gets or sets the version of the associated aggregate.
        /// </summary>
        public abstract TVersion Version
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} [Id = {Id}, Version = {Version}]";
    }
}
