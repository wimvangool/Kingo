using System;

namespace Kingo.Messaging.SampleApplication
{
    internal sealed class AddProductToCartCommand : Message
    {
        public Guid ShoppingCartId;
        public int ProductId;
        public int Quantity;

        public override Message Copy()
        {
            return new AddProductToCartCommand
            {
                ShoppingCartId = ShoppingCartId,
                ProductId = ProductId,
                Quantity = Quantity
            };
        }
    }
}
