using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents a controller that receives messages from a service-bus
    /// and dispatches those messages to a <see cref="IMicroProcessor" /> for further
    /// processing.
    /// </summary>
    public abstract class MicroServiceBusControllerBase : IHostedService
    {
        private readonly List<IHostedService> _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusControllerBase" /> class.
        /// </summary>
        protected MicroServiceBusControllerBase()
        {
            _endpoints = new List<IHostedService>();
        }

        /// <summary>
        /// Returns the processor that will process all the messages.
        /// </summary>
        protected abstract IMicroProcessor Processor
        {
            get;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            foreach (var endpoint in _endpoints)
            {
                _endpoints.Add(endpoint);
            }            
            return Task.WhenAll(_endpoints.Select(endpoint => endpoint.StartAsync(cancellationToken)));
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken) =>
            Task.WhenAll(_endpoints.Select(endpoint => endpoint.StopAsync(cancellationToken)));

        ///// <summary>
        ///// Creates and returns a new endpoint for the specified <paramref name="messageHandler" />.        
        ///// </summary>
        ///// <param name="messageHandler">The message-handler for which the endpoint will be created.</param>
        ///// <returns>A new endpoint.</returns>
        //protected abstract IHostedService CreateEndpointFor(MessageHandlerClass messageHandler);
    }
}
