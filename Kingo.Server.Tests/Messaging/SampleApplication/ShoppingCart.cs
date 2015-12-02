﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging.SampleApplication
{
    public sealed class ShoppingCart : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;         
        private readonly List<ShoppingCartItem> _items;
        private int _version;

        private ShoppingCart(ShoppingCartCreatedEvent @event)
            : base(NewEvent(@event))
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

        protected override int NextVersion(int version)
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

        public static ShoppingCart CreateShoppingCart(Guid shoppingCartId)
        {
            return new ShoppingCart(new ShoppingCartCreatedEvent
            {
                ShoppingCartId = shoppingCartId,
                ShoppingCartVersion = 1
            });            
        }
    }
}
