using System.ComponentModel.Server.SampleApplication.Messages;

namespace System.ComponentModel.Server.SampleApplication.MessageHandlers
{
    [MessageHandler(InstanceLifetime.PerUnitOfWork)]
    internal sealed class ShoppingCartHandler : IMessageHandler<AddProductToCartCommand>,
                                                IMessageHandler<CreateShoppingCartCommand>
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

        public void Handle(CreateShoppingCartCommand message)
        {
            _carts.Add(ShoppingCart.CreateShoppingCart(message.ShoppingCartId));            
        }

        public void Handle(AddProductToCartCommand message)
        {
            var shoppingCart = _carts.GetById(message.ShoppingCartId);

            shoppingCart.AddProduct(message.ProductId, message.Quantity);
        }
    }
}
