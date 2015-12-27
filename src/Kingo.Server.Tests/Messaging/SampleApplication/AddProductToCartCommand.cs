using System;

namespace Kingo.Messaging.SampleApplication
{
    internal sealed class AddProductToCartCommand : Message<AddProductToCartCommand>
    {
        public Guid ShoppingCartId;
        public int ProductId;
        public int Quantity;

        public override AddProductToCartCommand Copy()
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
