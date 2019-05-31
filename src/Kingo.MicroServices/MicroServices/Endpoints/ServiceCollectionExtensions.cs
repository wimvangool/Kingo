using System;
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
        /// <returns>A configured services-collection.</returns>
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
    }
}
