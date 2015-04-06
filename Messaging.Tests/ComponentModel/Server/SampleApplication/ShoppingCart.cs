using System.Collections.Generic;
using System.ComponentModel.Server.Domain;
using System.ComponentModel.Server.SampleApplication.Messages;
using System.Linq;

namespace System.ComponentModel.Server.SampleApplication
{
    public sealed class ShoppingCart : Aggregate<Guid, Int32Version>
    {
        private readonly Guid _id;
        private Int32Version _version;

        private readonly List<ShoppingCartItem> _items;

        private ShoppingCart(Guid id)
        {
            _id = id;
            _version = Int32Version.Zero;

            _items = new List<ShoppingCartItem>(2);
        }

        public override Guid Key
        {
            get { return _id; }
        }

        protected override Int32Version Version
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

            Write(new ProductAddedToCartEvent
            {
                ShoppingCartId = _id,
                ShoppingCartVersion = Int32Version.Increment(ref _version),
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

            cart.Write(new ShoppingCartCreatedEvent
            {
                ShoppingCartId = shoppingCartId,
                ShoppingCartVersion = Int32Version.Increment(ref cart._version)
            });
            return cart;
        }
    }
}
