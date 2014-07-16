using System;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal sealed class NotSupportedBus : IMessageProcessorBus
    {
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            throw new NotSupportedException();
        }

        public IConnection Connect<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class
        {
            throw new NotSupportedException();
        }

        public IConnection Connect<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {
            throw new NotSupportedException();
        }
    }
}
