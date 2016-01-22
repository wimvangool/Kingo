using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Threading;

namespace Kingo.Messaging.SampleApplication
{
    [Dependency(InstanceLifetime.Singleton)]
    public sealed class ShoppingCartRepository : SnapshotRepository<Guid, int, ShoppingCart>, IShoppingCartRepository
    {        
        private readonly Dictionary<Guid, ISnapshot<Guid, int>> _carts;
        private int _flushCount;

        public ShoppingCartRepository()
        {            
            _carts = new Dictionary<Guid, ISnapshot<Guid, int>>(4);
        }

        protected override ITypeToContractMap TypeToContractMap
        {
            get { return Domain.TypeToContractMap.Empty; }
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

        protected override Task FlushAsync(IWritableEventStream<Guid, int> domainEventStream)
        {
            Interlocked.Increment(ref _flushCount);

            return base.FlushAsync(domainEventStream);
        }

        protected override Task<ISnapshot<Guid, int>> SelectByKeyAsync(Guid key, ITypeToContractMap map)
        {
            return AsyncMethod.RunSynchronously(() => _carts[key]);
        }

        protected override Task InsertAsync(Snapshot<Guid, int> snapshot)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Add(snapshot.Value.Key, snapshot.Value));
        }

        protected override Task<bool> UpdateAsync(Snapshot<Guid, int> snapshot, int originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                _carts[snapshot.Value.Key] = snapshot.Value;
                return true;
            });            
        }        

        protected override Task DeleteAsync(Guid key, IWritableEventStream<Guid, int> domainEventStream)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Remove(key));
        }        
    }
}
