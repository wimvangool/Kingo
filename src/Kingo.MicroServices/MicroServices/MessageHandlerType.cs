using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    internal sealed class MessageHandlerType : MessageHandler
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

        #region [====== FromInstance ======]

        public static MessageHandlerType FromInstance(object messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }
            var component = new MicroProcessorComponent(messageHandler.GetType());
            var interfaces = MessageHandlerInterface.FromComponent(component).ToArray();
            return new MessageHandlerType(component, interfaces);
        }

        #endregion

        #region [====== FromComponents ======]               

        public static IEnumerable<MessageHandlerType> FromComponents(IEnumerable<MicroProcessorComponent> components)
        {                  
            foreach (var component in components)
            {
                if (IsMessageHandlerComponent(component, out var messageHandler))
                {
                    yield return messageHandler;
                }
            }            
        }

        internal static bool IsMessageHandlerComponent(Type type, out MessageHandlerType messageHandler)
        {
            if (CanBeCreatedFrom(type))
            {
                return IsMessageHandlerComponent(new MicroProcessorComponent(type), out messageHandler);
            }
            messageHandler = null;
            return false;
        }

        private static bool IsMessageHandlerComponent(MicroProcessorComponent component, out MessageHandlerType messageHandler)
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
