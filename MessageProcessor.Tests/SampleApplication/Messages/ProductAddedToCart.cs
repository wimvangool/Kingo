using System;
using YellowFlare.MessageProcessing.Aggregates;

namespace YellowFlare.MessageProcessing.SampleApplication.Messages
{
    internal sealed class ProductAddedToCart : IDomainEvent<Guid>
    {
        Guid IDomainEvent<Guid>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        int IDomainEvent<Guid>.AggregateVersion
        {
            get { return ShoppingCartVersion; }
        }

        public Guid ShoppingCartId;
        public int ShoppingCartVersion;
        public int ProductId;
        public int OldQuantity;
        public int NewQuantity;               
    }
}
