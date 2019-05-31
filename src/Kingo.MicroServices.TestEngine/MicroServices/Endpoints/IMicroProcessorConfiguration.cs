using System;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a service collection that is to be
    /// configured for a <see cref="IMicroProcessor" /> and its dependencies.
    /// </summary>
    public interface IMicroProcessorConfiguration
    {
        /// <summary>
        /// Configures and registers a <see cref="MicroProcessor" /> to use by the test-runner.
        /// </summary>                
        /// <param name="processorConfigurator">
        /// Optional configuration callback that can be used to configure the specific parts of the processor.
        /// </param>
        IServiceCollectionConfiguration Setup(Action<IMicroProcessorBuilder> processorConfigurator = null);

        /// <summary>
        /// Configures and registers a specific type of <see cref="IMicroProcessor" /> to use in this application or service.
        /// </summary>
        /// <typeparam name="TProcessor">Type of the processor to register.</typeparam>        
        /// <param name="processorConfigurator">
        /// Optional configuration callback that can be used to configure the specific parts of the processor.
        /// </param>
        IServiceCollectionConfiguration Setup<TProcessor>(Action<IMicroProcessorBuilder> processorConfigurator = null)
            where TProcessor : class, IMicroProcessor;
    }
}
