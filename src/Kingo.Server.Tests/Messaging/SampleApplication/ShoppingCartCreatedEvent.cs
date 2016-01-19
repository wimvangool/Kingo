using System;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging.SampleApplication
{
    internal sealed class ShoppingCartCreatedEvent : Message, IHasVersion<Guid, int>
    {
        public Guid ShoppingCartId;
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

        Guid IHasKey<Guid>.Key
        {
            get { return ShoppingCartId; }
        }

        int IHasVersion<Guid, int>.Version
        {
            get { return ShoppingCartVersion; }
        }

        public override Message Copy()
        {
            return new ShoppingCartCreatedEvent(this);
        }
    }
}
