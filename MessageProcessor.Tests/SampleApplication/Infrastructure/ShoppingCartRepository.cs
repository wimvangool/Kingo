using System;
using System.Collections.Generic;
using YellowFlare.MessageProcessing.Aggregates;

namespace YellowFlare.MessageProcessing.SampleApplication.Infrastructure
{
    internal sealed class ShoppingCartRepository : IShoppingCartRepository, IUnitOfWork
    {
        private readonly DomainEventBusAdapter<Guid> _domainEventBus;
        private readonly IUnitOfWorkController _controller;
        private readonly Dictionary<Guid, ShoppingCart> _carts;
        private int _flushCount;

        public ShoppingCartRepository(IUnitOfWorkController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            _domainEventBus = new DomainEventBusAdapter<Guid>(MessageProcessorTest.Processor.DomainEventBus);
            _controller = controller;
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
            _domainEventBus.PublishEvents(_carts.Values);
            _flushCount++;
        }        

        public void Add(ShoppingCart cart)
        {
            _controller.Enlist(this);

            _carts.Add((cart as IAggregate<Guid>).Key, cart);           
        }        

        public ShoppingCart GetById(Guid id)
        {
            _controller.Enlist(this);

            return _carts[id];
        }        
    }
}
