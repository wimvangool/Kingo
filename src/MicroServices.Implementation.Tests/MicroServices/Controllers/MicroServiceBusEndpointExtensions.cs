using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal static class MicroServiceBusEndpointExtensions
    {
        public static Task<IMessageHandlerOperationResult> InvokeAsync(this IMicroServiceBusEndpoint endpoint, object message, CancellationToken? token = null) =>
            endpoint.InvokeAsync(new MessageEnvelope(message, Guid.NewGuid().ToString()), token);
    }
}
