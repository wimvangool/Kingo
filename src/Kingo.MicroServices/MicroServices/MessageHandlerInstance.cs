using System;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance : MessageHandler, IEquatable<MessageHandlerInstance>
    {
        private readonly object _messageHandler;

        private MessageHandlerInstance(MessageHandlerType component, object messageHandler) :
            base(component)
        {
            _messageHandler = messageHandler;
        }

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;

        public override bool Equals(MicroProcessorComponent other) =>
            Equals(other as MessageHandlerInstance);

        public bool Equals(MessageHandlerInstance other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return ReferenceEquals(_messageHandler, other._messageHandler);
        }        

        public static bool IsMessageHandlerInstance(object messageHandler, out MessageHandlerInstance instance)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }
            if (MessageHandlerType.IsMessageHandlerComponent(messageHandler.GetType(), out var component))
            {
                instance = new MessageHandlerInstance(component, messageHandler);
                return true;
            }
            instance = null;
            return false;
        }
    }
}
