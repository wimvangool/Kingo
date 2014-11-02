using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal sealed class MessageProcessorBusConnectionForInterface<TMessage> : MessageProcessorBusConnection
        where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;

        public MessageProcessorBusConnectionForInterface(ICollection<MessageProcessorBusConnection> connection, IMessageHandler<TMessage> handler) : base(connection)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
        }

        public override void Handle<TPublished>(IMessageProcessor processor, TPublished message)
        {
            var messageToHandle = message as TMessage;
            if (messageToHandle == null)
            {
                return;
            }
            processor.Process(messageToHandle, _handler);
        }
    }
}
