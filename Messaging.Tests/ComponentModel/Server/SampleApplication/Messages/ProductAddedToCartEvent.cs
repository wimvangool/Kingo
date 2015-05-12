using System.ComponentModel.Server.Domain;

namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ProductAddedToCartEvent : Message<ProductAddedToCartEvent>, IAggregateRootEvent<Guid, int>
    {
        public Guid ShoppingCartId;
        public int ShoppingCartVersion;
        public int ProductId;
        public int OldQuantity;
        public int NewQuantity;

        Guid IAggregateRootEvent<Guid, int>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        int IAggregateRootEvent<Guid, int>.AggregateVersion
        {
            get { return ShoppingCartVersion; }
        }        

        public override ProductAddedToCartEvent Copy()
        {
            return new ProductAddedToCartEvent()
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
