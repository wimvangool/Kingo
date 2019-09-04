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

        public IEnumerable<HandleAsyncMethod<TMessage>> CreateMethodsFor<TMessage>(MicroProcessorOperationKinds operationKind, IServiceProvider serviceProvider) =>
            from factory in _methodFactories
            from method in factory.CreateMethodsFor<TMessage>(operationKind, serviceProvider)
            select method;
    }
}
