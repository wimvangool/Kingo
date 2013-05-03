using System;
using YellowFlare.MessageProcessing.SampleApplication.Messages;

namespace YellowFlare.MessageProcessing.SampleApplication.MessageHandlers
{
    internal sealed class ShoppingCartHandler : IExternalMessageHandler<AddProductToCart>,
                                                IExternalMessageHandler<CreateShoppingCart>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartHandler(IShoppingCartRepository shoppingCartRepository)
        {
            if (shoppingCartRepository == null)
            {
                throw new ArgumentNullException("shoppingCartRepository");
            }
            _shoppingCartRepository = shoppingCartRepository;
        }

        public void Handle(CreateShoppingCart message)
        {
            _shoppingCartRepository.Add(new ShoppingCart(message.ShoppingCartId));

            DomainEventBus.Publish(new ShoppingCartCreated
            {
                ShoppingCartId = message.ShoppingCartId
            });
        }

        public void Handle(AddProductToCart message)
        {
            var shoppingCart = _shoppingCartRepository.GetById(message.ShoppingCartId);

            shoppingCart.AddProduct(message.ProductId, message.Quantity);
        }
    }
}
