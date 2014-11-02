using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal abstract class MessageProcessorBusConnection : Connection
    {       
        private readonly ICollection<MessageProcessorBusConnection> _connections;        

        protected MessageProcessorBusConnection(ICollection<MessageProcessorBusConnection> connections)
        {            
            _connections = connections;                      
        }

        protected override bool IsOpen
        {
            get { return _connections.Contains(this); }
        }

        protected override void OpenConnection()
        {
            _connections.Add(this);
        }

        protected override void CloseConnection()
        {
            _connections.Remove(this);
        }

        public abstract void Handle<TPublished>(IMessageProcessor processor, TPublished message);
    }
}
