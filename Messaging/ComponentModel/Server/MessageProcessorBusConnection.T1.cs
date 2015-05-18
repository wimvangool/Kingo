using System.Collections.Generic;
using System.Diagnostics;

namespace System.ComponentModel.Server
{
    internal sealed class MessageProcessorBusConnection<TMessage> : Connection, IMessageProcessorBusConnection where TMessage : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ICollection<IMessageProcessorBusConnection> _connections;        
        private readonly IMessageHandler<TMessage> _handler;

        public MessageProcessorBusConnection(ICollection<IMessageProcessorBusConnection> connections, IMessageHandler<TMessage> handler)
        {                        
            _connections = connections;
            _handler = new MessageHandlerDecorator<TMessage>(handler);
        }

        internal MessageProcessorBusConnection(ICollection<IMessageProcessorBusConnection> connections, Action<TMessage> handler)
        {
            _connections = connections;
            _handler = (ActionDecorator<TMessage>) handler;
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

        public void Handle<TPublished>(IMessageProcessor processor, TPublished message) where TPublished : class, IMessage<TPublished>
        {
            var handler = _handler as IMessageHandler<TPublished>;
            if (handler == null)
            {
                return;
            }
            processor.Handle(message, handler);
        }
    }
}
