using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    public abstract class MicroServiceBusEndpoint<TMessageEnvelope> : HostedService
    {
        protected virtual Task HandleAsync(TMessageEnvelope message, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
