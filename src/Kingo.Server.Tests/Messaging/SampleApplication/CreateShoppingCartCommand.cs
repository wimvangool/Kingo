using System;

namespace Kingo.Messaging.SampleApplication
{
    internal class CreateShoppingCartCommand : Message
    {
        public Guid ShoppingCartId;

        public override Message Copy()
        {
            return new CreateShoppingCartCommand
            {
                ShoppingCartId = ShoppingCartId
            };
        }
    }
}
