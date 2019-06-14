using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    internal interface IHandleAsyncMethodFactory
    {
        IEnumerable<HandleAsyncMethodEndpoint> CreateMethodEndpoints(MicroProcessor processor);

        IEnumerable<HandleAsyncMethod<TMessage>> CreateMethodsFor<TMessage>(MicroProcessorOperationKinds operationKind, IServiceProvider serviceProvider);        
    }
}
