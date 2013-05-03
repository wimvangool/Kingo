using System;

namespace YellowFlare.MessageProcessing.SampleApplication.Messages
{
    internal sealed class ProductAddedToCart
    {
        public Guid ShoppingCartId
        {
            get;
            set;
        }

        public int ProductId
        {
            get;
            set;
        }

        public int OldQuantity
        {
            get;
            set;
        }

        public int NewQuantity
        {
            get;
            set;
        }
    }
}
