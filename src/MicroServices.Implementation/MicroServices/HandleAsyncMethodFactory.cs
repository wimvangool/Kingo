using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{        
    internal sealed class HandleAsyncMethodFactory : IHandleAsyncMethodFactory
    {                        
        private readonly IHandleAsyncMethodFactory[] _methodFactories;        
              
        public HandleAsyncMethodFactory(IEnumerable<IHandleAsyncMethodFactory> methodFactories)
        {
            _methodFactories = methodFactories.ToArray();            
        }                
        
        public override string ToString() =>
            $"{_methodFactories.Length} MessageHandler(s) registered";

        public IEnumerable<MicroServiceBusEndpoint> CreateMicroServiceBusEndpoints(MicroProcessor processor) =>
            from factory in _methodFactories
            from method in factory.CreateMicroServiceBusEndpoints(processor)
            select method;

        public IEnumerable<HandleAsyncMethod<TEvent>> CreateInternalEventBusEndpointsFor<TEvent>(IServiceProvider serviceProvider) =>
            from factory in _methodFactories
            from method in factory.CreateInternalEventBusEndpointsFor<TEvent>(serviceProvider)
            select method;
    }
}
