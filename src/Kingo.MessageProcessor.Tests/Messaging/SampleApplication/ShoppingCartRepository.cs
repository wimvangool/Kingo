using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Threading;

namespace Kingo.Messaging.SampleApplication
{
    [Dependency(MessageHandlerLifetime.Singleton)]
    public sealed class ShoppingCartRepository : SnapshotRepository<Guid, int, ShoppingCart>, IShoppingCartRepository
    {        
        private readonly Dictionary<Guid, IMemento<Guid, int>> _carts;
        private int _flushCount;

        public ShoppingCartRepository()
        {            
            _carts = new Dictionary<Guid, IMemento<Guid, int>>(4);
        }

        protected override ITypeToContractMap TypeToContractMap
        {
            get { return Messaging.TypeToContractMap.Empty; }
        }

        void IShoppingCartRepository.Add(ShoppingCart cart)
        {
            Add(cart);
        }

        Task<ShoppingCart> IShoppingCartRepository.GetById(Guid id)
        {
            return GetByKeyAsync(id);
        }

        public int FlushCount
        {
            get { return _flushCount; }
        }

        protected override Task FlushAsync(IDomainEventBus<Guid, int> eventBus)
        {
            Interlocked.Increment(ref _flushCount);

            return base.FlushAsync(eventBus);
        }

        protected override Task<IMemento<Guid, int>> SelectSnapshotByKeyAsync(Guid key, ITypeToContractMap map)
        {
            return AsyncMethod.RunSynchronously(() => _carts[key]);
        }

        protected override Task InsertAsync(SnapshotToSave<Guid, int> snapshotToSave)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Add(snapshotToSave.Value.Key, snapshotToSave.Value));
        }

        protected override Task<bool> UpdateAsync(SnapshotToSave<Guid, int> snapshotToSave, int originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                _carts[snapshotToSave.Value.Key] = snapshotToSave.Value;
                return true;
            });            
        }        

        protected override Task DeleteAsync(Guid key, IDomainEventBus<Guid, int> eventBus)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Remove(key));
        }        
    }
}
