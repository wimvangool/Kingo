using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{        
    internal sealed class MessageBusEndpointFactory : IMessageBusEndpointFactory
    {                        
        private readonly IMessageBusEndpointFactory[] _methodFactories;        
              
        public MessageBusEndpointFactory(IEnumerable<IMessageBusEndpointFactory> methodFactories)
        {
            _methodFactories = methodFactories.ToArray();            
        }                
        
        public override string ToString() =>
            $"{_methodFactories.Length} MessageHandler(s) registered";

        public IEnumerable<MicroServiceBusEndpoint> CreateMicroServiceBusEndpoints(MicroProcessor processor) =>
            from factory in _methodFactories
            from method in factory.CreateMicroServiceBusEndpoints(processor)
            select method;

        public IEnumerable<HandleAsyncMethod<TEvent>> CreateInternalEventBusEndpoints<TEvent>(IServiceProvider serviceProvider) =>
            from factory in _methodFactories
            from method in factory.CreateInternalEventBusEndpoints<TEvent>(serviceProvider)
            select method;
    }
}
