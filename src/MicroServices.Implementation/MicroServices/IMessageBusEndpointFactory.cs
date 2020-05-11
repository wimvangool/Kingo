using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    internal interface IMessageBusEndpointFactory
    {
        IEnumerable<MicroServiceBusEndpoint> CreateMicroServiceBusEndpoints(MicroProcessor processor);

        IEnumerable<HandleAsyncMethod<TEvent>> CreateInternalEventBusEndpoints<TEvent>(IServiceProvider serviceProvider);        
    }
}
