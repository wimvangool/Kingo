using System;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreatedEvent : Message<ShoppingCartCreatedEvent>, IVersionedObject<Guid, int>
    {
        public Guid ShoppingCartId;
        public int ShoppingCartVersion;

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
            return new ShoppingCartCreatedEvent()
            {
                ShoppingCartId = ShoppingCartId,
                ShoppingCartVersion = ShoppingCartVersion
            };
        }
    }
}
