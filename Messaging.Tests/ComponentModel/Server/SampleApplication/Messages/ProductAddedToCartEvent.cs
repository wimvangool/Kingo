using System.ComponentModel.Server.Domain;

namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class ProductAddedToCartEvent : Message<ProductAddedToCartEvent>, IAggregateEvent<Guid, Int32Version>
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
