using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Server
{
    internal sealed class MessageProcessorBusConnectionForAction<TMessage> : MessageProcessorBusConnection where TMessage : class
    {
        private readonly Action<TMessage> _action;

        public MessageProcessorBusConnectionForAction(ICollection<MessageProcessorBusConnection> subscriptions, Action<TMessage> action) : base(subscriptions)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
        }

        public override void Handle<TPublished>(IMessageProcessor processor, TPublished message)
        {
            var messageToHandle = message as TMessage;
            if (messageToHandle == null)
            {
                return;
            }
            processor.Process(messageToHandle, _action);
        }
    }
}
