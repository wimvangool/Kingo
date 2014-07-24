using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Server
{
    internal abstract class MessageHandlerPipelineDecorator<TMessage> : IMessageHandlerPipeline<TMessage> where TMessage : class
    {
        private readonly IMessageHandlerPipeline<TMessage> _handler;

        protected MessageHandlerPipelineDecorator(IMessageHandlerPipeline<TMessage> handler)
        {
            _handler = handler;
        }

        protected IMessageHandlerPipeline<TMessage> Handler
        {
            get { return _handler; }
        }

        public abstract void Handle(TMessage message);

        #region [====== Class Attributes ======]

        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _handler.TryGetClassAttributeOfType(out attribute);
        }

        public bool TryGetClassAttributeOfType<TAttribute>(bool includeInherited, out TAttribute attribute) where TAttribute : class
        {
            return _handler.TryGetClassAttributeOfType(includeInherited, out attribute);
        }

        public bool TryGetClassAttributeOfType(Type type, out object attribute)
        {
            return _handler.TryGetClassAttributeOfType(type, out attribute);
        }

        public bool TryGetClassAttributeOfType(Type type, bool includeInherited, out object attribute)
        {
            return _handler.TryGetClassAttributeOfType(type, includeInherited, out attribute);
        }

        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>(bool includeInherited = false)
        {
            return _handler.GetClassAttributesOfType<TAttribute>(includeInherited);
        }

        public IEnumerable<object> GetClassAttributesOfType(Type type, bool includeInherited = false)
        {
            return _handler.GetClassAttributesOfType(type, includeInherited);
        }

        #endregion

        #region [====== Method Attributes ======]

        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _handler.TryGetMethodAttributeOfType(out attribute);
        }

        public bool TryGetMethodAttributeOfType<TAttribute>(bool includeInherited, out TAttribute attribute) where TAttribute : class
        {
            return _handler.TryGetMethodAttributeOfType(includeInherited, out attribute);
        }

        public bool TryGetMethodAttributeOfType(Type type, out object attribute)
        {
            return _handler.TryGetMethodAttributeOfType(type, out attribute);
        }

        public bool TryGetMethodAttributeOfType(Type type, bool includeInherited, out object attribute)
        {
            return _handler.TryGetClassAttributeOfType(type, includeInherited, out attribute);
        }

        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>(bool includeInherited = false)
        {
            return _handler.GetMethodAttributesOfType<TAttribute>(includeInherited);
        }

        public IEnumerable<object> GetMethodAttributesOfType(Type type, bool includeInherited = false)
        {
            return _handler.GetMethodAttributesOfType(type, includeInherited);
        }

        #endregion
    }
}
