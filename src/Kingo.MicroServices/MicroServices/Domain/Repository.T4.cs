using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IRepository{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TSnapshot">Type of the snapshot of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>    
    public abstract class Repository<TKey, TVersion, TSnapshot, TAggregate> : RepositoryBase<TKey, TVersion, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
    {
        private readonly UnitOfWork<TKey, TVersion, TSnapshot, TAggregate> _unitOfWork;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, S, U, V}" /> class.
        /// </summary>
        /// <param name="serializationStrategy">
        /// Specifies the serialization strategy of this repository.
        /// </param>        
        protected Repository(SerializationStrategy serializationStrategy = null)
        {            
            _unitOfWork = new UnitOfWork<TKey, TVersion, TSnapshot, TAggregate>(this, serializationStrategy ?? SerializationStrategy.UseSnapshots());                     
        }              

        /// <inheritdoc />
        public override string ToString() =>
            _unitOfWork.ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        /// <inheritdoc />
        public override Task<TAggregate> GetByIdAsync(TKey id) =>
            _unitOfWork.GetByIdAsync(id);

        /// <inheritdoc />
        public override Task<TAggregate> GetByIdOrNullAsync(TKey id) =>
            _unitOfWork.GetByIdOrNullAsync(id);

        /// <inheritdoc />
        public override async Task<bool> AddAsync(TAggregate aggregate)
        {
            if (await _unitOfWork.AddAsync(aggregate).ConfigureAwait(false))
            {
                await UnitOfWork.EnlistAsync(this).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        internal Task OnAggregateModifiedAsync() =>
            UnitOfWork.EnlistAsync(this);

        /// <inheritdoc />
        public override async Task<bool> RemoveAsync(TAggregate aggregate)
        {
            if (await _unitOfWork.RemoveAsync(aggregate).ConfigureAwait(false))
            {
                await UnitOfWork.EnlistAsync(this).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public override async Task<bool> RemoveByIdAsync(TKey id)
        {
            if (await _unitOfWork.RemoveByIdAsync(id).ConfigureAwait(false))
            {
                await UnitOfWork.EnlistAsync(this).ConfigureAwait(false);
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
        protected internal abstract Task<AggregateReadSet> SelectByIdAsync(TKey id);        

        #endregion

        #region [====== Write Operations ======]

        /// <inheritdoc />
        public override bool RequiresFlush() =>
            _unitOfWork.RequiresFlush;

        /// <inheritdoc />
        protected internal override Task FlushAsync(bool keepAggregatesInMemory) =>
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
        protected internal abstract Task FlushAsync(IChangeSet<TKey, TVersion, TSnapshot> changeSet);                        

        #endregion
    }
}
