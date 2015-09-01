using System;

namespace Kingo.BuildingBlocks.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class AddProductToCartCommand : Message<AddProductToCartCommand>
    {
        public Guid ShoppingCartId;
        public int ProductId;
        public int Quantity;

        public override AddProductToCartCommand Copy()
        {
            return new AddProductToCartCommand()
            {
                ShoppingCartId = ShoppingCartId,
                ProductId = ProductId,
                Quantity = Quantity
            };
        }
    }
}
