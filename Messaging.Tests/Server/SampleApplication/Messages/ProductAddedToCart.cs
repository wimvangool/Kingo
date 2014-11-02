namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ProductAddedToCart : IAggregateEvent<Guid, Int32Version>
    {
        Guid IAggregateEvent<Guid, Int32Version>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        Int32Version IAggregateEvent<Guid, Int32Version>.AggregateVersion
        {
            get { return ShoppingCartVersion; }
        }

        public Guid ShoppingCartId;
        public Int32Version ShoppingCartVersion;
        public int ProductId;
        public int OldQuantity;
        public int NewQuantity;               
    }
}
