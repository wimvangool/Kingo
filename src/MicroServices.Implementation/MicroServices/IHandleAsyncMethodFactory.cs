using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    internal interface IHandleAsyncMethodFactory
    {
        IEnumerable<MicroServiceBusEndpoint> CreateMicroServiceBusEndpoints(MicroProcessor processor);

        IEnumerable<HandleAsyncMethod<TEvent>> CreateInternalEventBusEndpointsFor<TEvent>(IServiceProvider serviceProvider);        
    }
}
