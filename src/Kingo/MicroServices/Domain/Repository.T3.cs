using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IRepository{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public abstract class Repository<TKey, TVersion, TAggregate> : IRepository<TKey, TAggregate>, IUnitOfWorkResourceManager
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        private readonly UnitOfWork<TKey, TVersion, TAggregate> _unitOfWork;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, S, R}" /> class.
        /// </summary>
        /// <param name="serializationStrategy">Specifies the serialization strategy of this repository.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializationStrategy" /> is <c>null</c>.
        /// </exception>
        protected Repository(SerializationStrategy serializationStrategy)
        {
            _unitOfWork = new UnitOfWork<TKey, TVersion, TAggregate>(this, serializationStrategy);                     
        }              

        /// <inheritdoc />
        public override string ToString() =>
            _unitOfWork.ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        /// <inheritdoc />
        public virtual Task<TAggregate> GetByIdAsync(TKey id) =>
            _unitOfWork.GetByIdAsync(id);

        /// <inheritdoc />
        public virtual Task<TAggregate> GetByIdOrNullAsync(TKey id) =>
            _unitOfWork.GetByIdOrNullAsync(id);

        /// <inheritdoc />
        public virtual async Task<bool> AddAsync(TAggregate aggregate)
        {
            if (await _unitOfWork.AddAsync(aggregate))
            {
                await UnitOfWork.EnlistAsync(this);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public virtual async Task<bool> RemoveAsync(TAggregate aggregate)
        {
            if (await _unitOfWork.RemoveAsync(aggregate))
            {
                await UnitOfWork.EnlistAsync(this);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public virtual async Task<bool> RemoveByIdAsync(TKey id)
        {
            if (await _unitOfWork.RemoveByIdAsync(id))
            {
                await UnitOfWork.EnlistAsync(this);
                return true;
            }
            return false;
        }

        #endregion

        #region [====== Read Operations ======]                

        /// <summary>
        /// Loads and returns an aggregate's data from the data store.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to load.</param>
        /// <returns>
        /// The data of the aggregate, or <c>null</c> if the aggregate was not found.
        /// </returns>
        protected internal abstract Task<AggregateDataSet<TKey>> SelectByIdAsync(TKey id);        

        #endregion

        #region [====== Write Operations ======]

        /// <summary>
        /// Returns the unit of work that this repository is using to enlist itself in.
        /// </summary>
        protected virtual IUnitOfWork UnitOfWork =>
            MessageHandlerContext.Current?.UnitOfWork ?? MicroServices.UnitOfWork.None;                    

        /// <inheritdoc />
        public virtual object ResourceId =>
            null;

        /// <inheritdoc />
        public virtual bool RequiresFlush() =>
            _unitOfWork.RequiresFlush;

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
        protected Task FlushAsync(bool keepAggregatesInMemory) =>
            _unitOfWork.FlushAsync(keepAggregatesInMemory);

        /// <summary>
        /// Flushes all changes made in this session to the data store by inserting, updating and/or deleting several aggregates.
        /// </summary>
        /// <param name="changeSet">The change set containing all the changes made during this session.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="MessageHandlerException">
        /// The data store failed to accept the changes because a data constraint was violated or because a concurrency exception
        /// occurred.
        /// </exception>
        protected internal abstract Task FlushAsync(IChangeSet<TKey, TVersion> changeSet);                        

        #endregion
    }
}
