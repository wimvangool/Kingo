namespace System.ComponentModel.Server.SampleApplication
{
    internal interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        ShoppingCart GetById(Guid id);
    }
}
