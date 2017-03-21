using System;
using Kingo.Messaging.Validation;

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
