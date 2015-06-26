using System.Collections.Generic;
using System.ComponentModel.Server.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Server.SampleApplication.Infrastructure
{
    [MessageHandlerDependency(InstanceLifetime.Singleton)]
    public sealed class ShoppingCartRepository : Repository<ShoppingCart, Guid, int>, IShoppingCartRepository
    {        
        private readonly Dictionary<Guid, ShoppingCart> _carts;
        private int _flushCount;

        public ShoppingCartRepository()
        {            
            _carts = new Dictionary<Guid, ShoppingCart>(4);
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

        public override Task FlushAsync()
        {
            Interlocked.Increment(ref _flushCount);

            return base.FlushAsync();
        }

        protected override Task<ShoppingCart> SelectByKeyAsync(Guid key)
        {
            return AsyncMethod.RunSynchronously(() => _carts[key]);
        }

        protected override Task InsertAsync(ShoppingCart aggregate)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Add(aggregate.Id, aggregate));
        }

        protected override Task UpdateAsync(ShoppingCart aggregate, int originalVersion)
        {
            return AsyncMethod.RunSynchronously(() => _carts[aggregate.Id] = aggregate);            
        }        

        protected override Task DeleteAsync(Guid key)
        {
            return AsyncMethod.RunSynchronously(() => _carts.Remove(key));
        }        
    }
}
