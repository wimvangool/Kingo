using System;

namespace YellowFlare.MessageProcessing.Server.SampleApplication
{
    internal interface IShoppingCartRepository
    {
        void Add(ShoppingCart cart);

        ShoppingCart GetById(Guid id);
    }
}
