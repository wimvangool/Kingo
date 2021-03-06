﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    internal abstract class MicroProcessorConfigurationBase : IMicroProcessorConfiguration, IServiceCollectionConfiguration
    {
        public IServiceCollectionConfiguration Setup(Action<IMicroProcessorBuilder> processorConfigurator = null) =>
            Setup<MicroProcessor>(processorConfigurator);

        public abstract IServiceCollectionConfiguration Setup<TProcessor>(Action<IMicroProcessorBuilder> processorConfigurator = null)
            where TProcessor : class, IMicroProcessor;

        public abstract void Configure(Action<IServiceCollection> serviceConfigurator);

        public IMicroProcessor ResolveProcessor() =>
            ServiceProvider().GetRequiredService<IMicroProcessor>();

        public abstract IServiceProvider ServiceProvider();        
    }
}
