using System;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging.SampleApplication
{
    internal sealed class ProductAddedToCartEvent : DomainEvent<Guid, int>
    {
        [Key]
        public Guid ShoppingCartId;

        [Version]
        public int ShoppingCartVersion;

        public int ProductId;
        public int OldQuantity;
        public int NewQuantity;               

        public override Message Copy()
        {
            return new ProductAddedToCartEvent
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
