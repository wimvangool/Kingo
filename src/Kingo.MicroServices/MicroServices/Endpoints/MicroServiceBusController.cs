using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents a controller that receives messages from a service-bus
    /// and dispatches those messages to a <see cref="IMicroProcessor" /> for further
    /// processing.
    /// </summary>
    [MicroProcessorComponent(ServiceLifetime.Singleton)]
    public abstract class MicroServiceBusController : HostedEndpoint
    {                
        private IEnumerable<HostedEndpoint> _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusController" /> class.
        /// </summary>
        protected MicroServiceBusController()
        {            
            _endpoints = Enumerable.Empty<HostedEndpoint>();
        }

        /// <summary>
        /// Returns the processor that will process all the messages.
        /// </summary>
        protected abstract IMicroProcessor Processor
        {
            get;
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

            foreach (var endpoint in CreateHostedEndpoints())
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

        private IEnumerable<HostedEndpoint> CreateHostedEndpoints() =>
            Processor.CreateMethodEndpoints().Select(CreateHostedEndpoint);

        /// <summary>
        /// Creates and returns a new <see cref="HostedEndpoint"/> for the specified <paramref name="methodEndpoint"/>.
        /// </summary>
        /// <param name="methodEndpoint">A specific endpoint that is capable of processing messages of a specific type.</param>        
        /// <returns>A new <see cref="HostedEndpoint"/>.</returns>
        protected abstract HostedEndpoint CreateHostedEndpoint(HandleAsyncMethodEndpoint methodEndpoint);
    }
}
