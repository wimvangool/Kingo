using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.Domain
{
    internal sealed class RepositoryStub : SnapshotRepository<Guid, int, AggregateStub>
    {
        private readonly IMemento<Guid, int> _memento;

        internal RepositoryStub()
        {
            _memento = null;
        }

        internal RepositoryStub(AggregateStub aggregate)
        {
            _memento = aggregate;
        }

        protected override ITypeToContractMap TypeToContractMap
        {
            get { return Messaging.TypeToContractMap.Empty; }
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

        protected override Task FlushAsync(IDomainEventBus<Guid, int> eventBus)
        {
            WasEnlisted = false;

            return base.FlushAsync(eventBus);
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

        protected override Task<IMemento<Guid, int>> SelectSnapshotByKeyAsync(Guid key, ITypeToContractMap map)
        {
            Add(key, _selectedPrimaryKeys);

            return AsyncMethod.RunSynchronously(() =>
            {                              
                if (_memento != null && _memento.Key.Equals(key))
                {
                    return _memento;
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

        protected override Task<bool> UpdateAsync(SnapshotToSave<Guid, int> snapshotToSave, int originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                Add(snapshotToSave.Value.Key, _updatedKeys);
                return true;
            });
        }

        #endregion

        #region [====== InsertAsync ======]

        private readonly Dictionary<Guid, int> _insertedKeys = new Dictionary<Guid, int>();

        internal int InsertCountOf(Guid key)
        {
            return CountOf(key, _insertedKeys);
        }

        protected override Task InsertAsync(SnapshotToSave<Guid, int> snapshotToSave)
        {
            Add(snapshotToSave.Value.Key, _insertedKeys);

            return AsyncMethod.Void;
        }

        #endregion

        #region [====== DeleteAsync ======]

        private readonly Dictionary<Guid, int> _deletedKeys = new Dictionary<Guid, int>();

        internal int DeleteCountOf(Guid key)
        {
            return CountOf(key, _deletedKeys);
        }

        protected override Task DeleteAsync(Guid key, IDomainEventBus<Guid, int> eventBus)
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
