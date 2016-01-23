﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageHandler : IMessageHandlerWrapper
    {
        private readonly IMessageHandlerWrapper _nextHandler;
        private readonly MessageHandlerModule _nextModule;

        internal MessageHandler(IMessageHandlerWrapper nextHandler, MessageHandlerModule nextModule)
        {
            _nextHandler = nextHandler;
            _nextModule = nextModule;
        }

        public IMessage Message
        {
            get { return _nextHandler.Message; }
        }

        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _nextHandler.TryGetClassAttributeOfType(out attribute);
        }

        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _nextHandler.TryGetMethodAttributeOfType(out attribute);
        }

        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _nextHandler.GetClassAttributesOfType<TAttribute>();
        }

        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _nextHandler.GetMethodAttributesOfType<TAttribute>();
        }        

        public Task InvokeAsync()
        {
            return _nextModule.InvokeAsync(_nextHandler);
        }                      
    }
}
