namespace System.ComponentModel.Messaging.Server
{
    internal sealed class MessageProcessorBusRelay : IMessageProcessorBus
    {
        private readonly IMessageProcessorBus _messageProcessorBus;
        private readonly IDomainEventBus _domainEventBus;

        public MessageProcessorBusRelay(IMessageProcessorBus messageProcessorBus, IDomainEventBus domainEventBus)
        {
            _messageProcessorBus = messageProcessorBus;
            _domainEventBus = domainEventBus;
        }

        public IConnection Connect<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class
        {
            return _messageProcessorBus.Connect(action, openConnection);
        }

        public IConnection Connect<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class
        {
            return _messageProcessorBus.Connect(handler, openConnection);
        }

        public void Publish<TMessage>(TMessage message) where TMessage : class
        {
            _messageProcessorBus.Publish(message);
            _domainEventBus.Publish(message);
        }
    }
}
