using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFactoryInterface : MicroProcessorComponentInterface
    {
        private MessageIdFactoryInterface(Type type) :
            base(type)
        {
            MessageType = type.GetGenericArguments()[0];
        }

        /// <summary>
        /// The message type of the message that is handled by the <see cref="IMessageHandler{TMessage}"/>.
        /// </summary>
        public Type MessageType
        {
            get;
        }

        internal override string MethodName =>
            nameof(IMessageIdFactory<object>.GenerateMessageIdFor);

        #region [====== Factory Methods ======]

        public static IEnumerable<MessageIdFactoryInterface> FromComponent(MicroProcessorComponent component) =>
            from messageHandlerInterface in component.Type.GetInterfacesOfType(typeof(IMessageIdFactory<>))
            select new MessageIdFactoryInterface(messageHandlerInterface);

        public static MessageIdFactoryInterface FromType<TMessage>() =>
            new MessageIdFactoryInterface(typeof(IMessageIdFactory<TMessage>));

        #endregion
    }
}
