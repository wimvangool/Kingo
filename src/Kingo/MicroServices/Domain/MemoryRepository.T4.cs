using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Represents a repository where all aggregates are stored in memory.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier of the aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TSnapshot">Type of snapshot of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of the aggregate that is managed by this repository.</typeparam>    
    public class MemoryRepository<TKey, TVersion, TSnapshot, TAggregate> : Repository<TKey, TVersion, TSnapshot, TAggregate>, IReadOnlyCollection<TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
        where TSnapshot : class, ISnapshotOrEvent<TKey, TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion, TSnapshot>
    {
        private readonly MemoryDataStore<TKey, TVersion> _dataStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository{T, S, R, U}" /> class.
        /// </summary>              
        /// <param name="serializationStrategy">
        /// Specifies the serialization strategy of this repository.
        /// </param>               
        public MemoryRepository(SerializationStrategy serializationStrategy = null) :
            base(serializationStrategy)
        {
            _dataStore = new MemoryDataStore<TKey, TVersion>();
        }        

        /// <inheritdoc />
        public override string ToString() =>
            base.ToString() + $" (Count = {Count})";

        #region [====== IReadOnlyCollection<TAggregate> ======]

        /// <inheritdoc />
        public int Count =>
            _dataStore.Count;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<TAggregate> GetEnumerator()
        {
            foreach (var id in _dataStore.Keys)
            {
                yield return GetByIdAsync(id).Await();
            }            
        }

        #endregion

        #region [====== Read Operations ======]

        /// <inheritdoc />
        protected internal override Task<AggregateReadSet> SelectByIdAsync(TKey id) =>
            Task.FromResult(SelectById(id));

        private AggregateReadSet SelectById(TKey id)
        {
            if (_dataStore.TryGetValue(id, out var dataSet))
            {
                return dataSet;
            }
            return null;
        }

        #endregion

        #region [====== Write Operations ======]

        /// <inheritdoc />
        public override Task FlushAsync() =>
            FlushAsync(false);

        /// <inheritdoc />
        protected internal override Task FlushAsync(IChangeSet<TKey, TVersion, TSnapshot> changeSet) => AsyncMethod.Run(() =>
        {
            _dataStore.Flush(changeSet);
        });        

        #endregion
    }
}
