using System.ComponentModel.Messaging.Server.SampleApplication.Messages;

namespace System.ComponentModel.Messaging.Server.SampleApplication.MessageHandlers
{
    internal sealed class ShoppingCartHandler : IMessageHandler<AddProductToCart>,
                                                IMessageHandler<CreateShoppingCart>
    {
        private readonly IShoppingCartRepository _carts;

        public ShoppingCartHandler(IShoppingCartRepository carts)
        {
            if (carts == null)
            {
                throw new ArgumentNullException("carts");
            }
            _carts = carts;
        }

        public void Handle(CreateShoppingCart message)
        {
            _carts.Add(ShoppingCart.CreateShoppingCart(message.ShoppingCartId));            
        }

        public void Handle(AddProductToCart message)
        {
            var shoppingCart = _carts.GetById(message.ShoppingCartId);

            shoppingCart.AddProduct(message.ProductId, message.Quantity);
        }
    }
}
