using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a specific, closed version of the <see cref="IMessageHandler{TMessage}"/> interface.
    /// </summary>
    public sealed class MessageHandlerInterface : MessageHandlerOrQueryInterface<MessageHandlerComponent, HandleAsyncMethod>
    {
        private readonly Type _messageType;

        private MessageHandlerInterface(Type type, Type implementingType = null) : base(type, implementingType)
        {            
            _messageType = type.GetGenericArguments()[0];
        }

        /// <summary>
        /// The message type of the message that is handled by the <see cref="IMessageHandler{TMessage}"/>.
        /// </summary>
        public Type MessageType =>
            _messageType;

        internal override string MethodName =>
            nameof(IMessageHandler<object>.HandleAsync);

        internal override HandleAsyncMethod CreateMethod(MessageHandlerComponent component) =>
            new HandleAsyncMethod(component, this);

        #region [====== FromComponent & FromType ======]

        internal static IEnumerable<MessageHandlerInterface> FromComponent(MicroProcessorComponent component) =>
            from interfaceType in component.Type.GetInterfaces()
            from messageHandlerInterfaceType in interfaceType.GetInterfacesOfType(typeof(IMessageHandler<>))
            select new MessageHandlerInterface(messageHandlerInterfaceType, interfaceType);

        internal static MessageHandlerInterface FromType<TMessage>() =>
            new MessageHandlerInterface(typeof(IMessageHandler<TMessage>));

        #endregion
    }
}
