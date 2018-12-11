using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IRepository{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public abstract class Repository<TKey, TAggregate> : IRepository<TKey, TAggregate>, IUnitOfWorkResourceManager
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        private UnitOfWork<TKey, TAggregate> _unitOfWork;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, S}" /> class.
        /// </summary>
        /// <param name="serializationStrategy">Specifies the serialization strategy of this repository.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="serializationStrategy" /> does not specify a valid serialization strategy.
        /// </exception>
        protected Repository(AggregateSerializationStrategy serializationStrategy)
        {
            _unitOfWork = new UnitOfWork<TKey, TAggregate>(this, serializationStrategy);                     
        }

        /// <summary>
        /// Specifies the serialization strategy of this repository.
        /// </summary>
        public AggregateSerializationStrategy SerializationStrategy =>
            _unitOfWork.SerializationStrategy;       

        /// <inheritdoc />
        public override string ToString() =>
            _unitOfWork.ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        /// <inheritdoc />
        public virtual Task<TAggregate> GetByIdAsync(TKey id) =>
            _unitOfWork.GetByIdAsync(id);

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
        /// Loads and returns an aggregate from the data store.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to load.</param>
        /// <returns>The aggregate that was loaded from the data store.</returns>
        /// <exception cref="AggregateNotFoundException">
        /// No aggregate with the specified <paramref name="id" /> was found in the data store.
        /// </exception>
        protected internal async Task<TAggregate> SelectByIdAndRestoreAsync(TKey id)
        {
            var aggregateDataSet = await SelectByIdAsync(id);
            if (aggregateDataSet == null)
            {
                return null;
            }            
            try
            {
                return aggregateDataSet.RestoreAggregate<TAggregate>();
            }
            catch (Exception exception)
            {
                throw NewAggregateRestoreException(exception);
            }            
        }

        /// <summary>
        /// Loads and returns an aggregate's data from the data store.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to load.</param>
        /// <returns>
        /// The data of the aggregate, or <c>null</c> if the aggregate was not found.
        /// </returns>
        protected internal abstract Task<AggregateDataSet<TKey>> SelectByIdAsync(TKey id);        

        private static Exception NewAggregateRestoreException(Exception exception)
        {
            var messageFormat = ExceptionMessages.Repository_AggregateRestoreException;
            var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName());
            return new InternalServerErrorException(message, exception);
        }

        #endregion

        #region [====== Write Operations ======]

        /// <summary>
        /// Returns the unit of work that this repository is using to enlist itself in.
        /// </summary>
        protected virtual UnitOfWork UnitOfWork =>
            MessageHandlerContext.Current?.UnitOfWork ?? UnitOfWork.None;

        /// <summary>
        /// Returns the event bus that this repository uses to publish all the aggregate's events.
        /// </summary>
        protected internal virtual IEventBus EventBus =>
            MessageHandlerContext.Current?.EventBus;

        /// <inheritdoc />
        public virtual object ResourceId =>
            null;

        /// <inheritdoc />
        public virtual bool RequiresFlush() =>
            _unitOfWork.RequiresFlush();

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
        /// <exception cref="ConcurrencyException">
        /// A concurrency exception occurred.
        /// </exception>
        protected Task FlushAsync(bool keepAggregatesInMemory) =>
            Interlocked.Exchange(ref _unitOfWork, _unitOfWork.Commit(keepAggregatesInMemory)).FlushAsync();        

        /// <summary>
        /// Flushes all changes made in this session to the data store by inserting, updating and/or deleting several aggregates.
        /// </summary>
        /// <param name="changeSet">The change set containing all the changes made during this session.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="MessageHandlerException">
        /// The data store failed to accept the changes because a data constraint was violated or because a concurrency exception
        /// occurred.
        /// </exception>
        protected internal abstract Task FlushAsync(IChangeSet<TKey> changeSet);                        

        #endregion
    }
}
