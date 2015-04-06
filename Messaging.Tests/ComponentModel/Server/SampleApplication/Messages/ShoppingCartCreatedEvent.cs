using System.ComponentModel.Server.Domain;

namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreatedEvent : Message<ShoppingCartCreatedEvent>, IAggregateEvent<Guid, int>
    {
        public Guid ShoppingCartId;
        public int ShoppingCartVersion;

        Guid IAggregateEvent<Guid, int>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        int IAggregateEvent<Guid, int>.AggregateVersion
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
