using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal abstract class MessageProcessorBusSubscription : IDisposable
    {       
        private readonly ICollection<MessageProcessorBusSubscription> _subscriptions;        

        protected MessageProcessorBusSubscription(ICollection<MessageProcessorBusSubscription> subscriptions)
        {            
            _subscriptions = subscriptions;
            _subscriptions.Add(this);            
        }

        public void Dispose()
        {
            _subscriptions.Remove(this);
        }

        public abstract void Handle<TPublished>(IMessageProcessor processor, TPublished message);
    }
}
