using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a controller that receives messages from a service-bus
    /// and dispatches those messages to a <see cref="IMicroServiceBusProcessor" /> for further
    /// processing.
    /// </summary>
    [MicroProcessorComponent(ServiceLifetime.Singleton)]
    public abstract class MicroServiceBusController : IMicroServiceBus, IHostedService
    {
        // NB: When required, the controller needs to resolve a IEnumerable<IMicroServiceBus> to publish its events.
        private readonly IMicroServiceBusProcessor _processor;

        protected MicroServiceBusController(IMicroServiceBusProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        #region [====== IHostedService ======]

        public virtual Task StartAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        public virtual Task StopAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        #endregion

        #region [====== IMicroServiceBus ======]

        Task IMicroServiceBus.PublishAsync(IEnumerable<object> events) =>
            throw new NotImplementedException();

        Task IMicroServiceBus.PublishAsync(object @event) =>
            throw new NotImplementedException();

        #endregion
    }
}
