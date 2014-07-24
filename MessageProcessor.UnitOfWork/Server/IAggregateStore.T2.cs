using System;

namespace YellowFlare.MessageProcessing.Server
{
    /// <summary>
    /// Represents a DataStore for a specific type of Aggregate.
    /// </summary>
    /// <typeparam name="TKey">Key or identifier of the Aggregate.</typeparam>
    /// <typeparam name="TVersion">Version of the Aggregate.</typeparam>
    /// <typeparam name="TValue">Supported Aggregate-type.</typeparam>
    public interface IAggregateStore<in TKey, in TVersion, TValue>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
        where TValue : class, IAggregate<TKey, TVersion>
    {
        /// <summary>
        /// Attempts to retrieve a specific Aggregate instance by its key. Returns <c>true</c>
        /// if found; returns <c>false</c> otherwise.
        /// </summary>
        /// <param name="key">Key of the Aggregate instance to retrieve.</param>
        /// <param name="value">
        /// This argument will be set to the retrieved instance if retrieval was succesful. Will
        /// be set to <c>null</c> otherwise.
        /// </param>
        /// <returns>
        /// <c>True</c> if this store contains an instance with the specified <paramref name="key"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        bool TrySelect(TKey key, out TValue value);

        /// <summary>
        /// Inserts the specified instance into this DataStore.
        /// </summary>
        /// <param name="value">The instance to insert.</param>        
        void Insert(TValue value);

        /// <summary>
        /// Updates / overwrites a previous version with a new version.
        /// </summary>
        /// <param name="value">The instance to update.</param>
        /// <param name="originalVersion">The version to overwrite.</param>
        void Update(TValue value, TVersion originalVersion);

        /// <summary>
        /// Deletes / removes a certain instance from this DataStore.
        /// </summary>
        /// <param name="value">The instance to remove.</param>
        void Delete(TValue value);
    }
}
