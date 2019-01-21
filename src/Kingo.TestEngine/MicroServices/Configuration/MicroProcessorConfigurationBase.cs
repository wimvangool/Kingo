using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    internal abstract class MicroProcessorConfigurationBase : IMicroProcessorConfiguration, IServiceCollectionConfiguration
    {
        public IServiceCollectionConfiguration Add(Action<IMicroProcessorBuilder> processorConfigurator = null) =>
            Add<MicroProcessor>(processorConfigurator);

        public abstract IServiceCollectionConfiguration Add<TProcessor>(Action<IMicroProcessorBuilder> processorConfigurator = null)
            where TProcessor : class, IMicroProcessor;

        public abstract void Configure(Action<IServiceCollection> serviceConfigurator);

        public IMicroProcessor ResolveProcessor() =>
            ServiceProvider().GetRequiredService<IMicroProcessor>();

        public abstract IServiceProvider ServiceProvider();        
    }
}
