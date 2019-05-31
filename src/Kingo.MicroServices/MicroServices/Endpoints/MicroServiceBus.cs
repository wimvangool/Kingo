using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    public abstract class MicroServiceBus : HostedService, IMicroServiceBus
    {
        public virtual async Task PublishAsync(IEnumerable<object> messages)
        {
            throw new NotImplementedException();
        }

        public abstract Task PublishAsync(object message);
    }
}
