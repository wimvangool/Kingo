using System;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.SampleApplication
{
    [MessageHandler(InstanceLifetime.PerUnitOfWork)]
    internal sealed class ShoppingCartHandler : IMessageHandler<AddProductToCartCommand>,
                                                IMessageHandler<CreateShoppingCartCommand>,
                                                IMessageHandler<object>
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
                
        public Task HandleAsync(CreateShoppingCartCommand message)
        {
            return AsyncMethod.RunSynchronously(() =>
                _carts.Add(ShoppingCart.CreateShoppingCart(message.ShoppingCartId))
            );            
        }

        public async Task HandleAsync(AddProductToCartCommand message)
        {
            var shoppingCart = await _carts.GetById(message.ShoppingCartId);

            shoppingCart.AddProduct(message.ProductId, message.Quantity);           
        }

        public Task HandleAsync(object message)
        {
            return AsyncMethod.Void;
        }
    }
}
