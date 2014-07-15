using System;

namespace YellowFlare.MessageProcessing.Requests.Clients
{
    internal sealed class NotSupportedBus : IMessageProcessorBus
    {
        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            throw new NotSupportedException();
        }

        public IDisposable Subscribe<TMessage>(Action<TMessage> action) where TMessage : class
        {
            throw new NotSupportedException();
        }

        public IDisposable Subscribe<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {
            throw new NotSupportedException();
        }
    }
}
