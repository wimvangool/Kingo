using System.Threading.Tasks;

namespace System.ComponentModel.Server.SampleApplication
{
    public interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        Task<ShoppingCart> GetById(Guid id);
    }
}
