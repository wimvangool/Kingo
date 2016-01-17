using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Threading;

namespace Kingo.Messaging.SampleApplication
{
    [Dependency(InstanceLifetime.Singleton)]
    public sealed class ShoppingCartRepository : AggregateRootRepository<Guid, int, ShoppingCart>, IShoppingCartRepository
    {        
        private readonly Dictionary<Guid, ISnapshot<Guid, int>> _carts;
        private int _flushCount;

        public ShoppingCartRepository()
        {            
            _carts = new Dictionary<Guid, ISnapshot<Guid, int>>(4);
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

        protected override Task<ISnapshot<Guid, int>> SelectByKeyAsync(Guid key)
        {
            return AsyncMethod.RunSynchronously(() => _carts[key]);
        }

        protected override Task InsertAsync(ISnapshot<Guid, int> snapshot)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Add(snapshot.Key, snapshot));
        }

        protected override Task<bool> UpdateAsync(ISnapshot<Guid, int> snapshot, int originalVersion)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                _carts[snapshot.Key] = snapshot;
                return true;
            });            
        }        

        protected override Task DeleteAsync(Guid key, IWritableEventStream<Guid, int> domainEventStream)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Remove(key));
        }        
    }
}
