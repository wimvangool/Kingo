namespace System.ComponentModel.Server.SampleApplication
{
    public interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        ShoppingCart GetById(Guid id);
    }
}
