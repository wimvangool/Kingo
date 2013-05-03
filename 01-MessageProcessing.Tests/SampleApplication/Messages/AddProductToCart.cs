using System;

namespace YellowFlare.MessageProcessing.SampleApplication.Messages
{
    internal sealed class AddProductToCart
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

        public int Quantity
        {
            get;
            set;
        }
    }
}
