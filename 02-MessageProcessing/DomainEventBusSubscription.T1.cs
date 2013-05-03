using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal sealed class DomainEventBusSubscription<TMessage> : DomainEventBusSubscription, IInternalMessageHandler<TMessage>
        where TMessage : class
    {
        private readonly IInternalMessageHandler<TMessage> _handler;

        public DomainEventBusSubscription(ICollection<DomainEventBusSubscription> subscriptions, IInternalMessageHandler<TMessage> handler)
            : base(subscriptions)
        {
            _handler = handler;
        }

        public void Handle(TMessage message)
        {
            _handler.Handle(message);
        }
    }
}
