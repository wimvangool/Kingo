using System.Collections.Generic;
using System.ComponentModel.Server.Domain;
using System.ComponentModel.Server.SampleApplication.Messages;
using System.Linq;

namespace System.ComponentModel.Server.SampleApplication
{
    public sealed class ShoppingCart : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private int _version;

        private readonly List<ShoppingCartItem> _items;

        private ShoppingCart(Guid id)
        {
            _id = id;
            _version = 0;

            _items = new List<ShoppingCartItem>(2);
        }

        protected override Guid Key
        {
            get { return _id; }
        }

        protected override int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        protected override int Increment(int version)
        {
            return version + 1;
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

            Publish(version => new ProductAddedToCartEvent
            {
                ShoppingCartId = _id,
                ShoppingCartVersion = version,
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

            cart.Publish(version => new ShoppingCartCreatedEvent
            {
                ShoppingCartId = shoppingCartId,
                ShoppingCartVersion = version
            });
            return cart;
        }
    }
}
