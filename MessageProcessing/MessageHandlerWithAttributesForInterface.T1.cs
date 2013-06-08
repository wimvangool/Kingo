using System;
using System.Collections.Generic;
using System.Reflection;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageHandlerWithAttributesForInterface<TMessage> : MessageHandlerWithAttributes<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly Type _interfaceType;
        private readonly Lazy<MethodInfo> _handleMethod;

        public MessageHandlerWithAttributesForInterface(IMessageHandler<TMessage> handler, Type interfaceType)
        {
            _handler = handler;
            _interfaceType = interfaceType;
            _handleMethod = new Lazy<MethodInfo>(GetHandleMethod);            
        }
        
        public override void Handle(TMessage message)
        {
            _handler.Handle(message);
        }

        private MethodInfo GetHandleMethod()
        {            
            return _handler.GetType().GetInterfaceMap(_interfaceType).TargetMethods[0];
        }

        public override IEnumerable<object> GetClassAttributesOfType(Type type, bool includeInherited = false)
        {
            return _handler.GetType().GetCustomAttributes(type, includeInherited);
        }

        public override IEnumerable<object> GetMethodAttributesOfType(Type type, bool includeInherited = false)
        {
            return _handleMethod.Value.GetCustomAttributes(type, includeInherited);
        }
    }
}
