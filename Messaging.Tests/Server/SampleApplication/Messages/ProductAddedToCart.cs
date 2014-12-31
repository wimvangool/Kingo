namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ProductAddedToCart : Message<ProductAddedToCart>, IAggregateEvent<Guid, Int32Version>
    {
        public Guid ShoppingCartId;
        public Int32Version ShoppingCartVersion;
        public int ProductId;
        public int OldQuantity;
        public int NewQuantity;

        Guid IAggregateEvent<Guid, Int32Version>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        Int32Version IAggregateEvent<Guid, Int32Version>.AggregateVersion
        {
            get { return ShoppingCartVersion; }
        }        

        public override ProductAddedToCart Copy()
        {
            return new ProductAddedToCart()
            {
                ShoppingCartId = ShoppingCartId,
                ShoppingCartVersion = ShoppingCartVersion,
                ProductId = ProductId,
                OldQuantity = OldQuantity,
                NewQuantity = NewQuantity
            };
        }
    }
}
