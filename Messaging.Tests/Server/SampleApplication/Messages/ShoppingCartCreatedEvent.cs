namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreatedEvent : Message<ShoppingCartCreatedEvent>, IAggregateEvent<Guid, Int32Version>
    {
        public Guid ShoppingCartId;
        public Int32Version ShoppingCartVersion;

        Guid IAggregateEvent<Guid, Int32Version>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        Int32Version IAggregateEvent<Guid, Int32Version>.AggregateVersion
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
