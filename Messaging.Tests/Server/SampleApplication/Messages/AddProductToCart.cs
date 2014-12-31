namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal sealed class AddProductToCart : Message<AddProductToCart>
    {
        public Guid ShoppingCartId;
        public int ProductId;
        public int Quantity;

        public override AddProductToCart Copy()
        {
            return new AddProductToCart()
            {
                ShoppingCartId = ShoppingCartId,
                ProductId = ProductId,
                Quantity = Quantity
            };
        }
    }
}
