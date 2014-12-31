namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreated : Message<ShoppingCartCreated>, IAggregateEvent<Guid, Int32Version>
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

        public override ShoppingCartCreated Copy()
        {
            return new ShoppingCartCreated()
            {
                ShoppingCartId = ShoppingCartId,
                ShoppingCartVersion = ShoppingCartVersion
            };
        }
    }
}
