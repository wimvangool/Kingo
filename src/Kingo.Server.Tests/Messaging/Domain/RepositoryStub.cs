using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.Domain
{
    internal sealed class RepositoryStub : SnapshotRepository<Guid, int, AggregateStub>
    {
        private readonly AggregateStub _aggregate;

        internal RepositoryStub()
        {
            _aggregate = null;
        }

        internal RepositoryStub(AggregateStub aggregate)
        {
            _aggregate = aggregate;
        }

        internal bool WasEnlisted
        {
            get;
            private set;
        }

        protected override void OnAggregateSelected(AggregateStub aggregate)
        {
            WasEnlisted = true;

            base.OnAggregateSelected(aggregate);
        }

        protected override void OnAggregateAdded(AggregateStub aggregate)
        {
            WasEnlisted = true;

            base.OnAggregateAdded(aggregate);
        }

        protected override void OnAggregateRemoved<T>(T key)
        {
            WasEnlisted = true;

            base.OnAggregateRemoved(key);
        }

        protected override Task FlushAsync(IWritableEventStream<Guid, int> domainEventStream)
        {
            WasEnlisted = false;

            return base.FlushAsync(domainEventStream);
        }

        #region [====== SelectByKey ======]

        private readonly Dictionary<Guid, int> _selectedPrimaryKeys = new Dictionary<Guid, int>();
        private readonly Dictionary<int, int> _selectedSurrogateKeys = new Dictionary<int, int>();

        internal int SelectCountOf(Guid key)
        {
            return CountOf(key, _selectedPrimaryKeys);
        }

        internal int SelectCountOf(int key)
        {
            return CountOf(key, _selectedSurrogateKeys);
        }

        protected override Task<AggregateStub> SelectByKeyAsync(Guid key)
        {
            Add(key, _selectedPrimaryKeys);

            return AsyncMethod.RunSynchronously(() =>
            {                              
                if (_aggregate != null && _aggregate.Id.Equals(key))
                {
                    return _aggregate;
                }
                return null;
            });
        }      
  
        public Task<AggregateStub> GetByAlternateKeyAsync(int key)
        {
            return GetOrSelectByKeyAsync(key, aggregate => aggregate.AlternateKey, SelectByAlternateKey);
        }

        private Task<AggregateStub> SelectByAlternateKey(int key)
        {
            Add(key, _selectedSurrogateKeys);

            return AsyncMethod.RunSynchronously(() =>
            {
                if (_aggregate != null && _aggregate.AlternateKey.Equals(key))
                {
                    return _aggregate;
                }
                return null;
            });
        }

        #endregion

        #region [====== UpdateAsync ======]

        private readonly Dictionary<Guid, int> _updatedKeys = new Dictionary<Guid, int>();

        internal int UpdateCountOf(Guid key)
        {
            return CountOf(key, _updatedKeys);
        }

        protected override Task UpdateAsync(AggregateStub aggregate, int originalVersion)
        {
            Add(aggregate.Id, _updatedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        #region [====== InsertAsync ======]

        private readonly Dictionary<Guid, int> _insertedKeys = new Dictionary<Guid, int>();

        internal int InsertCountOf(Guid key)
        {
            return CountOf(key, _insertedKeys);
        }

        protected override Task InsertAsync(AggregateStub aggregate)
        {
            Add(aggregate.Id, _insertedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        #region [====== DeleteAsync ======]

        private readonly Dictionary<Guid, int> _deletedKeys = new Dictionary<Guid, int>();

        internal int DeleteCountOf(Guid key)
        {
            return CountOf(key, _deletedKeys);
        }

        protected override Task DeleteAsync(Guid key, IWritableEventStream<Guid, int> domainEventStream)
        {
            Add(key, _deletedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        private static int CountOf<TKey>(TKey key, IDictionary<TKey, int> keys)
        {
            int count;

            if (keys.TryGetValue(key, out count))
            {
                return count;
            }
            return 0;
        }

        private static void Add<TKey>(TKey key, IDictionary<TKey, int> keys)
        {
            int count;

            if (keys.TryGetValue(key, out count))
            {
                keys[key] = count + 1;
            }
            else
            {
                keys.Add(key, 1);
            }
        }
    }
}
