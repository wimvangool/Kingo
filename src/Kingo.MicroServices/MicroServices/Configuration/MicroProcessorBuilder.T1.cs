using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    internal sealed class MicroProcessorBuilder<TProcessor> : IMicroProcessorBuilder, IServiceCollectionConfigurator
        where TProcessor : class, IMicroProcessor
    {        
        public MicroProcessorBuilder()
        {
            ServiceBus = new MicroServiceBusBuilder();
            MessageHandlers = new MessageHandlerFactoryBuilder();
            Pipeline = new MicroProcessorPipelineFactoryBuilder();
        }

        public MicroServiceBusBuilder ServiceBus
        {
            get;
        }

        public MessageHandlerFactoryBuilder MessageHandlers
        {
            get;
        }

        public MicroProcessorPipelineFactoryBuilder Pipeline
        {
            get;
            set;
        }

        public void Configure(IServiceCollection services)
        {
            foreach (var configurator in Configurators())
            {
                configurator.Configure(services);
            }
            services.AddSingleton<IMicroProcessor, TProcessor>();
        }

        private IEnumerable<IServiceCollectionConfigurator> Configurators()
        {
            yield return ServiceBus;
            yield return MessageHandlers;
            yield return Pipeline;
        }
    }
}
