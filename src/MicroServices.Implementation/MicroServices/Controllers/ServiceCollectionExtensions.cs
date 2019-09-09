using System;
using System.Collections.Generic;
using System.Reflection;
using Kingo.Reflection;
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
        
        internal static IServiceCollection AddComponent(this IServiceCollection services, MicroProcessorComponent component, object instance = null)
        {                      
            // First, we add mappings from the serviceTypes - which are presumably interfaces or abstract classes -
            // to the component type by factory, so that at run-time they will all resolve the same instance if requested.
            // This will ensure that no matter through which (service) type the component is resolved, it will always
            // be resolved correctly (that is, with the appropriate lifetime).
            foreach (var serviceType in component.ServiceTypes)
            {
                services = services.AddTransient(serviceType, provider => provider.GetRequiredService(component.Type));
            }

            // Secondly, we add the component by its own type.
            // If no instance is provided, the component is registered with the specified lifetime.
            if (instance == null)
            {
                switch (component.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        return services.AddTransient(component.Type);
                    case ServiceLifetime.Scoped:
                        return services.AddScoped(component.Type);
                    case ServiceLifetime.Singleton:
                        return services.AddSingleton(component.Type);
                    default:
                        throw NewInvalidLifetimeSpecifiedException(component);
                }
            }
            // If an instance is provided, the lifetime is always implicitly set to singleton (because by definition, you always
            // resolve the same instance). However, the mapping itself uses Transient because that will trigger the provided factory
            // for each invocation.
            return services.AddTransient(component.Type, provider => instance);
        }

        private static Exception NewInvalidLifetimeSpecifiedException(MicroProcessorComponent component)
        {
            var messageFormat = ExceptionMessages.ServiceCollectionExtensions_InvalidComponentLifetime;
            var message = string.Format(messageFormat, component, component.Lifetime);
            return new ArgumentException(message, nameof(component));
        }

        #endregion
    }
}
