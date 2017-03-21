using System;
using Kingo.Messaging.Domain;
using Kingo.Messaging.Validation;

namespace Kingo.Messaging.SampleApplication
{
    internal sealed class ShoppingCartCreatedEvent : DomainEvent<Guid, int>
    {
        [Key]
        public Guid ShoppingCartId;

        [Version]
        public int ShoppingCartVersion;

        internal ShoppingCartCreatedEvent(Guid shoppingCartId)
        {
            ShoppingCartId = shoppingCartId;
            ShoppingCartVersion = 1;
        }

        private ShoppingCartCreatedEvent(ShoppingCartCreatedEvent message)
        {
            ShoppingCartId = message.ShoppingCartId;
            ShoppingCartVersion = message.ShoppingCartVersion;
        }        

        public override Message Copy()
        {
            return new ShoppingCartCreatedEvent(this);
        }
    }
}
