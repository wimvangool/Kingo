using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.SampleApplication.Infrastructure
{
    internal sealed class ShoppingCartRepository : IShoppingCartRepository, IUnitOfWork
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly Dictionary<Guid, ShoppingCart> _carts;
        private int _flushCount;

        public ShoppingCartRepository(IUnitOfWorkManager unitOfWorkManager)
        {
            if (unitOfWorkManager == null)
            {
                throw new ArgumentNullException("unitOfWorkManager");
            }
            _unitOfWorkManager = unitOfWorkManager;
            _carts = new Dictionary<Guid, ShoppingCart>(4);
        }

        public int FlushCount
        {
            get { return _flushCount; }
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return true;
        }

        void IUnitOfWork.Flush()
        {
            _flushCount++;
        }        

        public void Add(ShoppingCart cart)
        {
            _unitOfWorkManager.Enlist(this);
            _carts.Add(cart.Id, cart);           
        }        

        public ShoppingCart GetById(Guid id)
        {
            _unitOfWorkManager.Enlist(this);
            return _carts[id];
        }        
    }
}
