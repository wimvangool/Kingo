using System;
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Server.SampleApplication
{
    public interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        Task<ShoppingCart> GetById(Guid id);
    }
}
