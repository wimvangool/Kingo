using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension methods for <see cref="IServiceCollection" /> instances.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region [====== AddMicroProcessor ======]

        /// <summary>
        /// Configures and registers a <see cref="MicroProcessor" /> to use in this application or service.
        /// </summary>        
        /// <param name="services">A collection of services.</param>
        /// <param name="processorConfiguration">
        /// Optional configuration callback that can be used to configure the specific parts of the processor.
        /// </param>
        /// <returns>A configured services-collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is <c>null</c>.
        /// </exception>
        public static IServiceCollection AddMicroProcessor(this IServiceCollection services, Action<IMicroProcessorBuilder> processorConfiguration = null) =>
            services.AddMicroProcessor<MicroProcessor>(processorConfiguration);

        /// <summary>
        /// Configures and registers a specific type of <see cref="IMicroProcessor" /> to use in this application or service.
        /// </summary>
        /// <typeparam name="TProcessor">Type of the processor to register.</typeparam>
        /// <param name="services">A collection of services.</param>
        /// <param name="processorConfiguration">
        /// Optional configuration callback that can be used to configure the specific parts of the processor.
        /// </param>
        /// <returns>A configured service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is <c>null</c>.
        /// </exception>
        public static IServiceCollection AddMicroProcessor<TProcessor>(this IServiceCollection services, Action<IMicroProcessorBuilder> processorConfiguration = null) where TProcessor : MicroProcessor
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var builder = new MicroProcessorBuilder<TProcessor>();
            processorConfiguration?.Invoke(builder);
            return builder.BuildServiceCollection(services);
        }

        #endregion

        #region [====== AddComponent ======]
        
        internal static IServiceCollection AddComponents(this IServiceCollection services, IEnumerable<MicroProcessorComponent> components)
        {
            foreach (var component in components)
            {
                services = services.AddComponent(component);
            }
            return services;
        }

        internal static IServiceCollection AddComponent(this IServiceCollection services, MicroProcessorComponent component, object instance = null) =>
            component.AddTo(services, instance);

        #endregion
    }
}
