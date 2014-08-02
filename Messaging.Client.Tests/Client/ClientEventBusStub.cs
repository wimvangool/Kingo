using System.Collections.Generic;

namespace System.ComponentModel.Messaging.Client
{
    internal sealed class ClientEventBusStub : ClientEventBus
    {        
        private readonly List<object> _subscribers;
        private int _messageCount;

        public ClientEventBusStub()
        {            
            _subscribers = new List<object>();
        }

        public int SubscriberCount
        {
            get { return _subscribers.Count; }
        }

        public int MessageCount
        {
            get { return _messageCount; }
        }
        
        protected override void Subscribe(object subscriber)
        {
            _subscribers.Add(subscriber);
        }

        protected override void Unsubscribe(object subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public override void Publish<TMessage>(TMessage message)
        {
            _messageCount++;
        }

        public bool Contains(object subscriber)
        {
            return _subscribers.Contains(subscriber);
        }
    }
}
