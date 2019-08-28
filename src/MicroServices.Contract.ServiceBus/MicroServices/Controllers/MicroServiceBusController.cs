using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Collections.Generic;
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
        private readonly IMicroServiceBusProcessor _processor;
        private readonly IMicroServiceBus _bus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusController" /> class.
        /// </summary>
        /// <param name="processor">The processor that will be processing all commands and/or events.</param>
        /// <param name="bus">The bus that will be used to publish all new events.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> or <paramref name="bus"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusController(IMicroServiceBusProcessor processor, IMicroServiceBus bus)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        #region [====== IHostedService ======]

        // Starts the controller, connecting all endpoints of the processor to the service-bus to start
        // listening for events and processing them. While the controller has not been started, all
        // events that are published to it (possibly by the api or other controllers) are cached for a
        // short period of time and the publisher is made to wait. Should the timeout expire before the
        // controller is started, a timeout-exception should be thrown.
        //
        // NB: Extend IMicroServiceBus-interface with:
        //      PublishAsync(object @event, TimeSpan timeout);
        /// <inheritdoc />
        public virtual Task StartAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        /// <inheritdoc />
        public virtual Task StopAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        #endregion

        #region [====== IMicroServiceBus ======]

        Task IMicroServiceBus.PublishAsync(IEnumerable<object> events) =>
            PublishAsync(events ?? throw new ArgumentNullException(nameof(events)));

        /// <summary>
        /// Publishes all specified <paramref name="events" />.
        /// </summary>
        /// <param name="events">The events to publish.</param>
        protected virtual async Task PublishAsync(IEnumerable<object> events)
        {
            foreach (var @event in events.WhereNotNull())
            {
                await PublishAsync(@event);
            }
        }

        Task IMicroServiceBus.PublishAsync(object @event) =>
            PublishAsync(@event ?? throw new ArgumentNullException(nameof(@event)));

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        protected abstract Task PublishAsync(object @event);

        #endregion
    }
}
