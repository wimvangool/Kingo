using System;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging.SampleApplication
{
    public interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        Task<ShoppingCart> GetById(Guid id);
    }
}
