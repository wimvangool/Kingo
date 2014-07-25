namespace System.ComponentModel.Messaging.Server
{
    internal sealed class BufferedEvent<TMessage> : IBufferedEvent where TMessage : class
    {
        private readonly TMessage _domainEvent;

        public BufferedEvent(TMessage domainEvent)
        {
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            _domainEvent = domainEvent;
        }

        public void Publish(IDomainEventBus domainEventBus)
        {
            domainEventBus.Publish(_domainEvent);
        }
    }
}
