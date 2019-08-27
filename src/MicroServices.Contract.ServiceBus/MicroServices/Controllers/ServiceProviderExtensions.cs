using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension-methods for instance of type <see cref="IServiceProvider" />
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Resolves a <see cref="IMicroServiceBus" /> that will serve as the current service's main service bus.
        /// </summary>
        /// <param name="serviceProvider">A service provider.</param>
        /// <returns>
        /// A service bus that was resolved or constructed based on all <see cref="IMicroServiceBus"/> instances
        /// that were registered.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceProvider"/> is <c>null</c>.
        /// </exception>
        public static IMicroServiceBus GetMicroServiceBus(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            var microServiceBusCollection = serviceProvider.GetServices(typeof(IMicroServiceBus)).OfType<IMicroServiceBus>().ToArray();
            if (microServiceBusCollection.Length == 0)
            {
                return new MicroServiceBusStub();
            }
            if (microServiceBusCollection.Length == 1)
            {
                return microServiceBusCollection[0];
            }
            return new MicroServiceBusComposite(microServiceBusCollection);
        }
    }
}
