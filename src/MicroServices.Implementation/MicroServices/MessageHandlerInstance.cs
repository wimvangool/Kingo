using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerInstance : MessageHandlerComponent
    {
        private readonly object _messageHandler;

        private MessageHandlerInstance(MessageHandlerType component, object messageHandler) :
            base(component)
        {
            _messageHandler = messageHandler;
        }

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider) =>
            _messageHandler;

        internal override bool IsInternalEventBusEndpoint(MessageHandlerInterface @interface, out InternalEventBusEndpointAttribute attribute)
        {
            attribute = new InternalEventBusEndpointAttribute();
            return true;
        }

        public override string ToString() =>
            _messageHandler.GetType().FriendlyName();

        #region [====== Factory Methods ======]

        public static bool IsMessageHandlerInstance(object messageHandler, out MessageHandlerInstance instance)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }
            if (MessageHandlerType.IsMessageHandler(messageHandler.GetType(), out var component))
            {
                instance = new MessageHandlerInstance(component, messageHandler);
                return true;
            }
            instance = null;
            return false;
        }

        #endregion
    }
}
