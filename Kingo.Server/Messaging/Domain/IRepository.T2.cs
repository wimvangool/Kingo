using System;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents a generic repository of a certain type of aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the primary key of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate,</typeparam>
    public interface IRepository<in TKey, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IKeyedObject<TKey>
    {
        /// <summary>
        /// Retrieves the aggregate with the specified <paramref name="key"/> from the repository.
        /// </summary>
        /// <param name="key">Key (id) of the aggregate to retrieve.</param>
        /// <returns>A task that will yield the aggregate.</returns>
        /// <exception cref="AggregateNotFoundByKeyException{T}">
        /// No aggregate with the specified <paramref name="key"/> was found in the repository.
        /// </exception>
        Task<TAggregate> GetByIdAsync(TKey key);

        /// <summary>
        /// Adds the specified <paramref name="aggregate"/> to the repository so that it can later be persisted.
        /// </summary>
        /// <param name="aggregate">The aggregate to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="DuplicateKeyException{T}">
        /// Another aggregate with the same key is already present in the repository.
        /// </exception>
        void Add(TAggregate aggregate);

        /// <summary>
        /// Removes the aggregate with the specified <paramref name="key"/> from the repository, so that
        /// it can later be deleted.
        /// </summary>
        /// <param name="key">Key (id) of the aggregate to remove.</param>
        void RemoveById(TKey key);
    }
}
