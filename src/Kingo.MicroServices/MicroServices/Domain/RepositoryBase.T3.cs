using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IRepository{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public abstract class RepositoryBase<TKey, TVersion, TAggregate> : IRepository<TKey, TAggregate>, IUnitOfWorkResourceManager
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        internal RepositoryBase() { }        

        #region [====== IRepository<TKey, TAggregate> ======]

        /// <inheritdoc />
        public abstract Task<TAggregate> GetByIdAsync(TKey id);

        /// <inheritdoc />
        public abstract Task<TAggregate> GetByIdOrNullAsync(TKey id);

        /// <inheritdoc />
        public abstract Task<bool> AddAsync(TAggregate aggregate);

        /// <inheritdoc />
        public abstract Task<bool> RemoveAsync(TAggregate aggregate);

        /// <inheritdoc />
        public abstract Task<bool> RemoveByIdAsync(TKey id);

        #endregion

        #region [====== Write Methods ======]

        /// <summary>
        /// Returns the unit of work that this repository is using to enlist itself in.
        /// </summary>
        protected virtual IUnitOfWork UnitOfWork =>
            MessageHandlerContext.Current?.UnitOfWork ?? MicroServices.UnitOfWork.None;

        /// <inheritdoc />
        public virtual object ResourceId =>
            null;

        /// <inheritdoc />
        public abstract bool RequiresFlush();            

        /// <inheritdoc />
        public virtual Task FlushAsync() =>
            FlushAsync(true);

        /// <summary>
        /// Flushes any pending changes to the underlying infrastructure.
        /// </summary>
        /// <param name="keepAggregatesInMemory">
        /// Indicates whether or not this repository should keep all aggregates that have been retrieved, added and/or updated
        /// in its internal cache after the flush operation has been completed, so that following read and write operations
        /// are potentially faster.
        /// </param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="MessageHandlerException">
        /// The data store failed to accept the changes because a data constraint was violated or because a concurrency exception
        /// occurred.
        /// </exception>
        protected internal abstract Task FlushAsync(bool keepAggregatesInMemory);            

        #endregion
    }
}
