using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerInstance<TMessage> : IMessageHandlerOrQueryWrapper, IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly ClassAndMethodAttributeProvider _attributeProvider;

        internal MessageHandlerInstance(IMessageHandler<TMessage> handler)
        {
            _handler = handler;
            _attributeProvider = new ClassAndMethodAttributeProvider(handler.GetType(), typeof(IMessageHandler<TMessage>));            
        }

        internal MessageHandlerInstance(IMessageHandler<TMessage> handler, Type classType, Type interfaceType)
        {
            _handler = handler;
            _attributeProvider = new ClassAndMethodAttributeProvider(classType, interfaceType);
        }

        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _attributeProvider.TryGetClassAttributeOfType(out attribute);
        }

        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _attributeProvider.TryGetMethodAttributeOfType(out attribute);
        }

        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _attributeProvider.GetClassAttributesOfType<TAttribute>();
        }

        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _attributeProvider.GetMethodAttributesOfType<TAttribute>();
        }

        public Task HandleAsync(TMessage message)
        {
            return _handler.HandleAsync(message);
        }        
    }
}
