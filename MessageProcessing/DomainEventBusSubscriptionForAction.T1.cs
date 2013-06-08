using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal sealed class DomainEventBusSubscriptionForAction<TMessage> : DomainEventBusSubscription where TMessage : class
    {
        private readonly Action<TMessage> _action;

        public DomainEventBusSubscriptionForAction(ICollection<DomainEventBusSubscription> subscriptions, Action<TMessage> action)
            : base(subscriptions)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
        }

        public override void Handle<TPublished>(MessageProcessor processor, TPublished message)
        {
            var messageToHandle = message as TMessage;
            if (messageToHandle == null)
            {
                return;
            }
            processor.Handle(messageToHandle, _action, MessageSources.DomainEventBus);
        }
    }
}
