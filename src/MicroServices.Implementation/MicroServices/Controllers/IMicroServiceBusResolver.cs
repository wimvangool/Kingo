using System;

namespace Kingo.MicroServices.Controllers
{
    internal interface IMicroServiceBusResolver
    {
        IMicroServiceBus ResolveMicroServiceBus(IServiceProvider serviceProvider);
    }
}
