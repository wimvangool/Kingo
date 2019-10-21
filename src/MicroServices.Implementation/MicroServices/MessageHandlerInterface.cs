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
        private MessageHandlerInterface(Type type) :
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
            nameof(IMessageHandler<object>.HandleAsync);

        internal override HandleAsyncMethod CreateMethod(MessageHandlerComponent component) =>
            new HandleAsyncMethod(component, this);

        #region [====== FromComponent & FromType ======]

        internal static IEnumerable<MessageHandlerInterface> FromComponent(MicroProcessorComponent component) =>
            from messageHandlerInterface in component.Type.GetInterfacesOfType(typeof(IMessageHandler<>))
            select new MessageHandlerInterface(messageHandlerInterface);

        internal static MessageHandlerInterface FromType<TMessage>() =>
            new MessageHandlerInterface(typeof(IMessageHandler<TMessage>));

        #endregion
    }
}
