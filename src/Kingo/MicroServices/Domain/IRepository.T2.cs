using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a generic repository for a certain type of aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public interface IRepository<in TKey, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        /// <summary>
        /// Retrieves the aggregate with the specified <paramref name="id"/> from the repository.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to retrieve.</param>
        /// <returns>The aggregate with the specified <paramref name="id"/>.</returns>
        /// <exception cref="AggregateNotFoundException">
        /// No aggregate with the specified <paramref name="id"/> was found in the repository.
        /// </exception>
        Task<TAggregate> GetByIdAsync(TKey id);

        /// <summary>
        /// Retrieves the aggregate with the specified <paramref name="id"/> from the repository,
        /// or returns <c>null</c> if the aggregate was not found.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to retrieve.</param>
        /// <returns>
        /// The aggregate with the specified <paramref name="id"/> or <c>null</c> if the aggregate was
        /// not found.
        /// </returns>
        Task<TAggregate> GetByIdOrNullAsync(TKey id);

        /// <summary>
        /// Adds the specified <paramref name="aggregate"/> to the repository.
        /// </summary>
        /// <param name="aggregate">The aggregate to add.</param>
        /// <returns>
        /// <c>true</c> if the aggregate was newly added to the repository; 
        /// <c>false</c> if the specified <paramref name="aggregate"/> was already present in the repository.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="DuplicateKeyException">
        /// Another aggregate with the same identifier (key) is already present in the repository.
        /// </exception>
        Task<bool> AddAsync(TAggregate aggregate);

        /// <summary>
        /// Removes the aggregate with the specified <paramref name="id"/> from the repository.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to remove.</param>
        /// <returns>
        /// <c>true</c> if the aggregates was removed from the repository;
        /// <c>false</c> if no aggregate with the specified <paramref name="id"/> was found in the data store.
        /// </returns>        
        Task<bool> RemoveByIdAsync(TKey id);
    }
}
