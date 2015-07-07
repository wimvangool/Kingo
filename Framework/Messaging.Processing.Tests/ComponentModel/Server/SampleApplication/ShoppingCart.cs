using System;
using System.Collections.Generic;
using System.Linq;
using Syztem.ComponentModel.Server.Domain;
using Syztem.ComponentModel.Server.SampleApplication.Messages;

namespace Syztem.ComponentModel.Server.SampleApplication
{
    public sealed class ShoppingCart : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private int _version;

        private readonly List<ShoppingCartItem> _items;

        private ShoppingCart(Guid id)
        {
            _id = id;            
            _items = new List<ShoppingCartItem>(2);
        }

        public override Guid Id
        {
            get { return _id; }
        }

        protected override int Version
        {
            get { return _version; }
            set { SetVersion(ref _version, value); }
        }

        protected override int NewVersion()
        {
            return _version + 1;
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

            Publish((id, version) => new ProductAddedToCartEvent
            {
                ShoppingCartId = id,
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

            cart.Publish((id, version) => new ShoppingCartCreatedEvent
            {
                ShoppingCartId = id,
                ShoppingCartVersion = version
            });
            return cart;
        }
    }
}
