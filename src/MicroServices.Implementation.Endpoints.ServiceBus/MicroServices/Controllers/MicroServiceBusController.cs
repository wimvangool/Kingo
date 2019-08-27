using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a controller that receives messages from a service-bus
    /// and dispatches those messages to a <see cref="IMicroProcessor" /> for further
    /// processing.
    /// </summary>
    [MicroProcessorComponent(ServiceLifetime.Singleton)]
    public abstract class MicroServiceBusController : IHostedService
    {
        public virtual Task StartAsync(CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();

        public virtual Task StopAsync(CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
    }
}
