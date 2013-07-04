using System;
using System.Collections.Generic;
using System.Linq;
using YellowFlare.MessageProcessing.Aggregates;
using YellowFlare.MessageProcessing.SampleApplication.Messages;

namespace YellowFlare.MessageProcessing.SampleApplication
{
    internal sealed class ShoppingCart : BufferedEventAggregate<Guid>
    {
        private readonly Guid _id;
        private AggregateVersion _version;

        private readonly List<ShoppingCartItem> _items;

        private ShoppingCart(Guid id)
        {
            _id = id;
            _version = AggregateVersion.Zero;

            _items = new List<ShoppingCartItem>(2);
        }

        protected override Guid Key
        {
            get { return _id; }
        }

        protected override AggregateVersion Version
        {
            get { return _version; }
        }

        public void AddProduct(int productId, int quantity)
        {
            ShoppingCartItem item;

            if (!TryGetItem(productId, out item))
            {
                _items.Add(item = new ShoppingCartItem(productId));
            }
            int oldQuantity = item.Quantity;            

            item.AddQuantity(quantity);

            Write(new ProductAddedToCart
            {
                ShoppingCartId = _id,
                ShoppingCartVersion = AggregateVersion.Increment(ref _version).ToInt32(),
                ProductId = productId,
                OldQuantity = oldQuantity,
                NewQuantity = item.Quantity
            });
        }

        private bool TryGetItem(int productId, out ShoppingCartItem item)
        {
            return (item = _items.SingleOrDefault(i => i.ProductId == productId)) != null;            
        }

        public static ShoppingCart CreateShoppingCart(Guid shoppingCartId)
        {
            var cart = new ShoppingCart(shoppingCartId);

            cart.Write(new ShoppingCartCreated
            {
                ShoppingCartId = shoppingCartId,
                ShoppingCartVersion = AggregateVersion.Increment(ref cart._version).ToInt32()
            });
            return cart;
        }
    }
}
