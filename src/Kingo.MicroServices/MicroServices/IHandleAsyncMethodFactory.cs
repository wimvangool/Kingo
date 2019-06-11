using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    internal interface IHandleAsyncMethodFactory
    {        
        IEnumerable<HandleAsyncMethod<TMessage>> CreateMethodsFor<TMessage>(MicroProcessorOperationKinds operationKind, IServiceProvider serviceProvider);        
    }
}
