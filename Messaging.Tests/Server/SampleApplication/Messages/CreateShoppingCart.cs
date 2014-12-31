namespace System.ComponentModel.Server.SampleApplication.Messages
{
    internal class CreateShoppingCart : Message<CreateShoppingCart>
    {
        public Guid ShoppingCartId;

        public override CreateShoppingCart Copy()
        {
            return new CreateShoppingCart()
            {
                ShoppingCartId = ShoppingCartId
            };
        }
    }
}
