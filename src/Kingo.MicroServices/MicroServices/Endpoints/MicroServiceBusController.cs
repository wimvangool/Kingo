using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents a controller that receives messages from a service-bus
    /// and dispatches those messages to a <see cref="IMicroProcessor" /> for further
    /// processing.
    /// </summary>
    public abstract class MicroServiceBusController : HostedEndpoint
    {        
        private readonly IMicroProcessor _processor;
        private IEnumerable<HostedEndpoint> _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusController" /> class.
        /// </summary>
        protected MicroServiceBusController(IMicroProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _endpoints = Enumerable.Empty<HostedEndpoint>();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                foreach (var endpoint in _endpoints)
                {
                    endpoint.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override async Task ConnectAsync(CancellationToken cancellationToken) =>
            _endpoints = await ConnectAllEndpointsAsync(cancellationToken);

        private async Task<IEnumerable<HostedEndpoint>> ConnectAllEndpointsAsync(CancellationToken cancellationToken)
        {
            // Upon start, the controller creates all necessary endpoints and attempts
            // to start them. However, if cancellation is requested or an exception is thrown
            // during the startup process, all endpoints that were already started are stopped
            // again and the rest of the startup process is aborted.
            var endpoints = new List<HostedEndpoint>();

            foreach (var endpoint in CreateEndpointsFor(_processor))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await DisconnectAsync(endpoints, CancellationToken.None);
                    return Enumerable.Empty<HostedEndpoint>();
                }
                try
                {
                    await endpoint.StartAsync(cancellationToken);
                }
                catch
                {
                    await DisconnectAsync(endpoints, CancellationToken.None);
                    throw;
                }
                endpoints.Add(endpoint);
            }
            return endpoints;
        }

        /// <inheritdoc />
        protected override Task DisconnectAsync(CancellationToken cancellationToken) =>
            DisconnectAsync(Interlocked.Exchange(ref _endpoints, Enumerable.Empty<HostedEndpoint>()), cancellationToken);

        private static Task DisconnectAsync(IEnumerable<HostedEndpoint> endpoints, CancellationToken cancellationToken) =>
            Task.WhenAll(endpoints.Select(endpoint => DisconnectAsync(endpoint, cancellationToken)));

        private static async Task DisconnectAsync(HostedEndpoint endpoint, CancellationToken cancellationToken)
        {
            try
            {
                await endpoint.StopAsync(cancellationToken);
            }
            finally
            {
                endpoint.Dispose();
            }                       
        }

        private IEnumerable<HostedEndpoint> CreateEndpointsFor(IMicroProcessor processor)
        {
            foreach (var messageHandler in processor.CreateEndpoints())
            {
                if (TryCreateEndpointFor(messageHandler, out var endpoint))
                {
                    yield return endpoint;
                }
            }
        }

        protected abstract bool TryCreateEndpointFor(IMessageHandlerEndpoint messageHandler, out HostedEndpoint endpoint);
    }
}
