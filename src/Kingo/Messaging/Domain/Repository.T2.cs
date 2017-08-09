using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IRepository{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public abstract class Repository<TKey, TAggregate> : IRepository<TKey, TAggregate>, IUnitOfWork
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        private UnitOfWork<TKey, TAggregate> _unitOfWork;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T, S}" /> class.
        /// </summary>
        protected Repository()
        {
            _unitOfWork = new UnitOfWork<TKey, TAggregate>(this);                     
        }

        /// <inheritdoc />
        public override string ToString() =>
            _unitOfWork.ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        /// <inheritdoc />
        public virtual Task<TAggregate> GetByIdAsync(TKey id) =>
            _unitOfWork.GetByIdAsync(id);

        /// <inheritdoc />
        public virtual Task<bool> AddAsync(TAggregate aggregate) =>
            _unitOfWork.AddAsync(aggregate);

        /// <inheritdoc />
        public Task<bool> RemoveByIdAsync(TKey id) =>
            _unitOfWork.RemoveByIdAsync(id);

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
            var aggregateData = await SelectByIdAsync(id);
            if (aggregateData == null)
            {
                return null;
            }            
            try
            {
                return (TAggregate) aggregateData.Snapshot.RestoreAggregate(aggregateData.Events);
            }
            catch (Exception exception)
            {
                throw NewAggregateRestoreException(Context.Messages.Current?.Message, exception);
            }            
        }

        /// <summary>
        /// Loads and returns an aggregate's data from the data store.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to load.</param>
        /// <returns>
        /// The data of the aggregate, or <c>null</c> if the aggregate was not found.
        /// </returns>
        protected internal abstract Task<AggregateData<TKey>> SelectByIdAsync(TKey id);        

        private static Exception NewAggregateRestoreException(object failedMessage, Exception exception)
        {
            var messageFormat = ExceptionMessages.Repository_AggregateRestoreException;
            var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName());
            return new InternalServerErrorException(failedMessage, message, exception);
        }

        #endregion

        #region [====== Write Operations ======]

        /// <summary>
        /// Returns the current context.
        /// </summary>
        protected internal virtual IMicroProcessorContext Context =>
            MicroProcessorContext.Current;

        /// <summary>
        /// Returns the resource identifier of this repository. This identifier is used to determine
        /// which <see cref="IUnitOfWork" /> implementations can be flushed in parrallel (different id's) and which must
        /// be flushed sequentially (equal id's).
        /// </summary>
        protected internal virtual object ResourceId =>
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
        /// <exception cref="InternalProcessorException">
        /// The data store failed to accept the changes because a data constraint was violated or because a concurrency exception
        /// occurred.
        /// </exception>
        protected internal abstract Task FlushAsync(IChangeSet<TKey> changeSet);                        

        #endregion
    }
}
