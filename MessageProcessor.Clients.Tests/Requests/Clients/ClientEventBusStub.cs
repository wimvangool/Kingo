using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal sealed class ClientEventBusStub : ClientEventBus
    {
        private readonly IMessageProcessorBus _eventBus;
        private readonly List<object> _subscribers;

        public ClientEventBusStub(IMessageProcessorBus eventBus)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException("eventBus");
            }
            _eventBus = eventBus;
            _subscribers = new List<object>();
        }

        public int SubscriberCount
        {
            get { return _subscribers.Count; }
        }

        protected override IMessageProcessorBus EventBus
        {
            get { return _eventBus; }
        }

        protected override void Subscribe(object subscriber)
        {
            _subscribers.Add(subscriber);
        }

        protected override void Unsubscribe(object subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public override void Publish(object message) { }

        public bool Contains(object subscriber)
        {
            return _subscribers.Contains(subscriber);
        }
    }
}
