using System;
using System.Collections.Generic;
using System.Linq;
using YellowFlare.MessageProcessing.SampleApplication.Messages;

namespace YellowFlare.MessageProcessing.SampleApplication
{
    internal sealed class ShoppingCart
    {
        private readonly Guid _id;        
        private readonly List<ShoppingCartItem> _items;

        public ShoppingCart(Guid id)
        {
            _id = id;           
            _items = new List<ShoppingCartItem>(2);
        }

        public Guid Id
        {
            get { return _id; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }
            var other = obj as ShoppingCart;

            if (ReferenceEquals(other, null))
            {
                return false;                
            }
            return _id.Equals(other._id);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
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

            DomainEventBus.Publish(new ProductAddedToCart
            {
                ShoppingCartId = _id,
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
