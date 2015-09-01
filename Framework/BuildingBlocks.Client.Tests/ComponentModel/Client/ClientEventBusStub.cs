using System.Collections.Generic;

namespace Kingo.BuildingBlocks.ComponentModel.Client
{
    internal sealed class ClientEventBusStub : ClientEventBus
    {        
        private readonly List<object> _subscribers;
        private int _messageCount;

        public ClientEventBusStub() : base(new SynchronousContext())
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
        
        protected internal override void Subscribe(object subscriber)
        {
            _subscribers.Add(subscriber);
        }

        protected internal override void Unsubscribe(object subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public override void Publish(object message)
        {
            _messageCount++;
        }

        public bool Contains(object subscriber)
        {
            return _subscribers.Contains(subscriber);
        }
    }
}
