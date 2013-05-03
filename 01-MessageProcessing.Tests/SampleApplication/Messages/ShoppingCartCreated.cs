using System;

namespace YellowFlare.MessageProcessing.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreated
    {
        public Guid ShoppingCartId
        {
            get;
            set;
        }
    }
}
