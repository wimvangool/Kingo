using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageProcessorBusSubscriptionForInterface<TMessage> : MessageProcessorBusSubscription
        where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;

        public MessageProcessorBusSubscriptionForInterface(ICollection<MessageProcessorBusSubscription> subscriptions, IMessageHandler<TMessage> handler)
            : base(subscriptions)
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
            processor.Handle(messageToHandle, _handler);
        }
    }
}
