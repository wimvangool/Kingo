using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
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
        public static IServiceCollection AddMicroProcessor<TProcessor>(this IServiceCollection services, Action<IMicroProcessorBuilder> processorConfiguration = null) where TProcessor : class, IMicroProcessor
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            // We override the current directory to the directory of the calling assembly, since
            // the calling assembly is typically the main assembly where all configuration takes place,
            // and we want its location to serve as the reference location.
            using (TypeSet.OverrideCurrentDirectory(Assembly.GetCallingAssembly()))
            {
                var builder = new MicroProcessorBuilder<TProcessor>();
                processorConfiguration?.Invoke(builder);
                return builder.BuildServiceCollection(services);                
            }
        }

        #endregion

        #region [====== AddComponent ======]        
            
        /// <summary>
        /// Adds the specified <paramref name="component" /> to the service collection based on its configuration.
        /// If <paramref name="serviceTypes" /> is specified, an additional mapping from each service type to
        /// the type of the component is added to the collection.
        /// </summary>
        /// <param name="services">A collection of services.</param>
        /// <param name="component">The component to add.</param>
        /// <param name="serviceTypes">Optional collection of service types for which additional mappings are added.</param>
        /// <returns>A configured service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> or <paramref name="component"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="component"/> specifies an invalid service lifetime.
        /// </exception>
        public static IServiceCollection AddComponent(this IServiceCollection services, MicroProcessorComponent component, IEnumerable<Type> serviceTypes = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (serviceTypes == null)
            {
                serviceTypes = Enumerable.Empty<Type>();
            }
            // First, we add mappings from the serviceTypes - which are presumably interfaces or abstract classes -
            // to the component type by factory, so that at run-time they will all resolve the same instance if requested.
            foreach (var serviceType in serviceTypes)
            {
                services = services.AddTransient(serviceType, provider => provider.GetRequiredService(component.Type));
            }

            // Secondly, we check whether or not the component has already been registered to prevent registering the same
            // component twice.
            if (services.Any(service => service.ServiceType == component.Type))
            {
                return services;
            }

            // If the component wasn't added yet, we add it here with the specified lifetime.
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

        private static Exception NewInvalidLifetimeSpecifiedException(MicroProcessorComponent component)
        {
            var messageFormat = ExceptionMessages.ServiceCollectionExtensions_InvalidComponentLifetime;
            var message = string.Format(messageFormat, component, component.Lifetime);
            return new ArgumentException(message, nameof(component));
        }

        #endregion
    }
}
