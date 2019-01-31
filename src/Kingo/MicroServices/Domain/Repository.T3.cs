using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IRepository{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>
    public abstract class Repository<TKey, TVersion, TAggregate> : RepositoryBase<TKey, TVersion, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        #region [====== AggregateRootDecoratorRepository ======]

        private sealed class AggregateRootDecoratorRepository : Repository<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>, AggregateRootDecorator<TKey, TVersion, TAggregate>>
        {
            private readonly Repository<TKey, TVersion, TAggregate> _repository;

            public AggregateRootDecoratorRepository(Repository<TKey, TVersion, TAggregate> repository) :
                base(SerializationStrategy.UseEvents())
            {
                _repository = repository;
            }            

            protected internal override async Task<AggregateReadSet> SelectByIdAsync(TKey id)
            {
                var events = await _repository.SelectByIdAsync(id);
                if (events == null)
                {
                    return null;
                }
                return new AggregateReadSet(events);
            }

            protected override IUnitOfWork UnitOfWork =>
                _repository.UnitOfWork;

            public override object ResourceId =>
                _repository.ResourceId;

            protected internal override Task FlushAsync(IChangeSet<TKey, TVersion, ISnapshotOrEvent<TKey, TVersion>> changeSet) =>
                _repository.FlushAsync(new ChangeSetDecorator<TKey,TVersion, ISnapshotOrEvent<TKey, TVersion>>(changeSet));
        }

        #endregion

        private readonly AggregateRootDecoratorRepository _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TKey, TVersion, TAggregate}" /> class.
        /// </summary>
        protected Repository()
        {
            _implementation = new AggregateRootDecoratorRepository(this);
        }

        /// <inheritdoc />
        public override string ToString() =>
            _implementation.ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        /// <inheritdoc />
        public override async Task<TAggregate> GetByIdAsync(TKey id) =>
            await _implementation.GetByIdAsync(id);

        /// <inheritdoc />
        public override async Task<TAggregate> GetByIdOrNullAsync(TKey id) => 
            await _implementation.GetByIdOrNullAsync(id);

        /// <inheritdoc />
        public override Task<bool> AddAsync(TAggregate aggregate) =>
            _implementation.AddAsync(new AggregateRootDecorator<TKey, TVersion, TAggregate>(aggregate));

        /// <inheritdoc />
        public override Task<bool> RemoveAsync(TAggregate aggregate) =>
            _implementation.RemoveAsync(new AggregateRootDecorator<TKey, TVersion, TAggregate>(aggregate));

        /// <inheritdoc />
        public override Task<bool> RemoveByIdAsync(TKey id) =>
            _implementation.RemoveByIdAsync(id);

        #endregion

        #region [====== Read Operations ======]                

        /// <summary>
        /// Loads and returns an aggregate's events from the data store.
        /// </summary>
        /// <param name="id">Identifier of the aggregate to load.</param>
        /// <returns>
        /// The events of the aggregate, or <c>null</c> if the aggregate was not found.
        /// </returns>
        protected internal abstract Task<IEnumerable<ISnapshotOrEvent>> SelectByIdAsync(TKey id);

        #endregion

        #region [====== IUnitOfWorkResourceManager ======]        

        public override bool RequiresFlush() =>
            _implementation.RequiresFlush();

        protected internal override Task FlushAsync(bool keepAggregatesInMemory) =>
            _implementation.FlushAsync(keepAggregatesInMemory);

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
