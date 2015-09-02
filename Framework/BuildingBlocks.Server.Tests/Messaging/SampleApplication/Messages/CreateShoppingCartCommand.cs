using System;

namespace Kingo.BuildingBlocks.Messaging.SampleApplication.Messages
{
    internal class CreateShoppingCartCommand : Message<CreateShoppingCartCommand>
    {
        public Guid ShoppingCartId;

        public override CreateShoppingCartCommand Copy()
        {
            return new CreateShoppingCartCommand()
            {
                ShoppingCartId = ShoppingCartId
            };
        }
    }
}
