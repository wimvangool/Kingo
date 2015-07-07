using System;

namespace Syztem.ComponentModel.Server.SampleApplication.Messages
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
