using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging.SampleApplication
{
    public sealed class ShoppingCart : AggregateRoot
    {
        private readonly Guid _id;         
        private readonly List<ShoppingCartItem> _items;
        private int _version;

        public ShoppingCart(Guid shoppingCartId)
            : this(new ShoppingCartCreatedEvent(shoppingCartId)) { }

        private ShoppingCart(ShoppingCartCreatedEvent @event)
            : base(@event)
        {
            _id = @event.ShoppingCartId;
            _version = @event.ShoppingCartVersion;
            _items = new List<ShoppingCartItem>(2);
        }

        public override Guid Id
        {
            get { return _id; }
        }

        protected override int Version
        {
            get { return _version; }
            set { _version = value; }
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

            Publish(new ProductAddedToCartEvent
            {
                ShoppingCartId = _id,
                ShoppingCartVersion = NextVersion(),
                ProductId = productId,
                OldQuantity = oldQuantity,
                NewQuantity = item.Quantity
            });
        }

        private bool TryGetItem(int productId, out ShoppingCartItem item)
        {
            return (item = _items.SingleOrDefault(i => i.ProductId == productId)) != null;            
        }               
    }
}
