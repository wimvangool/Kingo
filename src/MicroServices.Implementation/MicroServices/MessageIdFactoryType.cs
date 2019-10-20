using System;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFactoryType : MessageIdFactoryComponent
    {
        private MessageIdFactoryType(MicroProcessorComponent component, params MessageIdFactoryInterface[] interfaces) :
            base(component, interfaces) { }

        #region [====== Factory Methods ======]

        public static MessageIdFactoryType FromInstance<TMessage>(IMessageIdFactory<TMessage> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            var component = MicroProcessorComponent.FromInstance(factory);
            var interfaces = new [] { MessageIdFactoryInterface.FromType<TMessage>() };
            return new MessageIdFactoryType(component, interfaces);
        }

        public static bool IsMessageIdFactory(MicroProcessorComponent component, out MessageIdFactoryType factory)
        {
            var interfaces = MessageIdFactoryInterface.FromComponent(component).ToArray();
            if (interfaces.Length == 0)
            {
                factory = null;
                return false;
            }
            factory = new MessageIdFactoryType(component, interfaces);
            return true;
        }

        #endregion
    }
}
