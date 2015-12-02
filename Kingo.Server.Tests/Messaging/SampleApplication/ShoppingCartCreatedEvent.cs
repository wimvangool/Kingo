using System;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging.SampleApplication
{
    internal sealed class ShoppingCartCreatedEvent : Message<ShoppingCartCreatedEvent>, IVersionedObject<Guid, int>
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

        Guid IKeyedObject<Guid>.Key
        {
            get { return ShoppingCartId; }
        }

        int IVersionedObject<Guid, int>.Version
        {
            get { return ShoppingCartVersion; }
        }

        public override ShoppingCartCreatedEvent Copy()
        {
            return new ShoppingCartCreatedEvent(this);
        }
    }
}
