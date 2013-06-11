using System;
using System.Collections.Generic;
using System.Reflection;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageHandlerPipelineForClass<TMessage> : MessageHandlerPipeline<TMessage> where TMessage : class
    {
        private readonly MessageHandlerFactory _factory;
        private readonly Type _classType;
        private readonly Lazy<IMessageHandler<TMessage>> _handler;
        private readonly Lazy<MethodInfo> _handleMethod;

        public MessageHandlerPipelineForClass(MessageHandlerFactory factory, Type classType)
        {
            _factory = factory;
            _classType = classType;
            _handler = new Lazy<IMessageHandler<TMessage>>(CreateMessageHandler);
            _handleMethod = new Lazy<MethodInfo>(GetHandleMethod);
        }

        private IMessageHandler<TMessage> CreateMessageHandler()
        {
            return (IMessageHandler<TMessage>) _factory.CreateMessageHandler(_classType);
        }

        private MethodInfo GetHandleMethod()
        {
            return _classType.GetInterfaceMap(typeof(IMessageHandler<TMessage>)).TargetMethods[0];
        }

        public override void Handle(TMessage message)
        {
            _handler.Value.Handle(message);
        }

        public override IEnumerable<object> GetClassAttributesOfType(Type type, bool includeInherited = false)
        {
            return _classType.GetCustomAttributes(type, includeInherited);
        }

        public override IEnumerable<object> GetMethodAttributesOfType(Type type, bool includeInherited = false)
        {
            return _handleMethod.Value.GetCustomAttributes(type, includeInherited);
        }
    }
}
