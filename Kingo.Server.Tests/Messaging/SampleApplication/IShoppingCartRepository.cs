using System;
using System.Threading.Tasks;

namespace Kingo.Messaging.SampleApplication
{
    public interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        Task<ShoppingCart> GetById(Guid id);
    }
}
