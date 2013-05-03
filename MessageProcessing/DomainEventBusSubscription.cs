using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal abstract class DomainEventBusSubscription : IDisposable
    {       
        private readonly ICollection<DomainEventBusSubscription> _subscriptions;        

        protected DomainEventBusSubscription(ICollection<DomainEventBusSubscription> subscriptions)
        {            
            _subscriptions = subscriptions;
            _subscriptions.Add(this);            
        }

        public void Dispose()
        {
            _subscriptions.Remove(this);
        }           
    }
}
