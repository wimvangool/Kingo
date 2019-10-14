using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    /// <summary>
    /// Represents a type that implements one or more variations of the <see cref="IMessageHandler{TMessage}"/> interface.
    /// </summary>
    public sealed class MessageHandlerType : MessageHandler
    {        
        private MessageHandlerType(MicroProcessorComponent component, MessageHandlerInterface[] interfaces) :
            base(component, interfaces) { }        

        internal override object ResolveMessageHandler(IServiceProvider serviceProvider)
        {
            var messageHandler = serviceProvider?.GetService(Type);
            if (messageHandler == null)
            {
                throw NewCouldNotResolveMessageHandlerException(Type);
            }
            return messageHandler;
        }

        private static Exception NewCouldNotResolveMessageHandlerException(Type messageHandlerType)
        {
            var messageFormat = ExceptionMessages.MessageHandlerClass_CouldNotResolveMessageHandler;
            var message = string.Format(messageFormat, messageHandlerType.FriendlyName());
            return new InvalidOperationException(message);
        }

        #region [====== Factory Methods ======]

        internal new static MessageHandlerType FromInstance(object messageHandler)
        {            
            var component = MicroProcessorComponent.FromInstance(messageHandler);
            var interfaces = MessageHandlerInterface.FromComponent(component).ToArray();
            return new MessageHandlerType(component, interfaces);
        }

        internal static bool IsMessageHandler(Type type, out MessageHandlerType messageHandler)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsMessageHandler(component, out messageHandler);
            }
            messageHandler = null;
            return false;
        }

        internal static bool IsMessageHandler(MicroProcessorComponent component, out MessageHandlerType messageHandler)
        {
            var interfaces = MessageHandlerInterface.FromComponent(component).ToArray();
            if (interfaces.Length == 0)
            {
                messageHandler = null;
                return false;
            }
            messageHandler = new MessageHandlerType(component, interfaces);
            return true;                                   
        }                      

        #endregion        
    }
}
