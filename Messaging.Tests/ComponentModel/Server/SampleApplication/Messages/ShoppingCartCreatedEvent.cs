using System.ComponentModel.Server.Domain;

namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreatedEvent : Message<ShoppingCartCreatedEvent>, IAggregateRootEvent<Guid, int>
    {
        public Guid ShoppingCartId;
        public int ShoppingCartVersion;

        Guid IAggregateRootEvent<Guid, int>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        int IAggregateRootEvent<Guid, int>.AggregateVersion
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
