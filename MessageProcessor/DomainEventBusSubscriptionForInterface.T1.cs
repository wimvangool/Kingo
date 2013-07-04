using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal sealed class DomainEventBusSubscriptionForInterface<TMessage> : DomainEventBusSubscription
        where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;

        public DomainEventBusSubscriptionForInterface(ICollection<DomainEventBusSubscription> subscriptions, IMessageHandler<TMessage> handler)
            : base(subscriptions)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            _handler = handler;
        }

        public override void Handle<TPublished>(MessageProcessor processor, TPublished message)
        {
            var messageToHandle = message as TMessage;
            if (messageToHandle == null)
            {
                return;
            }
            processor.Handle(messageToHandle, _handler, MessageSources.DomainEventBus);
        }
    }
}
