using System.Collections.Generic;

namespace System.ComponentModel.Messaging.Server
{
    internal abstract class MessageProcessorBusConnection : Connection
    {       
        private readonly ICollection<MessageProcessorBusConnection> _subscriptions;        

        protected MessageProcessorBusConnection(ICollection<MessageProcessorBusConnection> subscriptions)
        {            
            _subscriptions = subscriptions;                      
        }

        protected override bool IsOpen
        {
            get { return _subscriptions.Contains(this); }
        }

        protected override void OpenConnection()
        {
            _subscriptions.Add(this);
        }

        protected override void CloseConnection()
        {
            _subscriptions.Remove(this);
        }

        public abstract void Handle<TPublished>(IMessageProcessor processor, TPublished message);
    }
}
