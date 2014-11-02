namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class AddProductToCart
    {
        public Guid ShoppingCartId;
        public int ProductId;
        public int Quantity;        
    }
}
