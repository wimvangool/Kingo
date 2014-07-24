using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Client
{
    internal sealed class ClientEventBusStub : ClientEventBus
    {        
        private readonly List<object> _subscribers;

        public ClientEventBusStub()
        {            
            _subscribers = new List<object>();
        }

        public int SubscriberCount
        {
            get { return _subscribers.Count; }
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
