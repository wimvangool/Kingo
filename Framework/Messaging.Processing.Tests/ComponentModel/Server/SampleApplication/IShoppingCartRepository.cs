using System;
using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server.SampleApplication
{
    public interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        Task<ShoppingCart> GetById(Guid id);
    }
}
