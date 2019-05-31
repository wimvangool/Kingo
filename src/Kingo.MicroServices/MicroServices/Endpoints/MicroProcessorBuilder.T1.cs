using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroProcessorBuilder<TProcessor> : IMicroProcessorBuilder, IServiceCollectionBuilder
        where TProcessor : class, IMicroProcessor
    {        
        public MicroProcessorBuilder()
        {            
            Components = new MicroProcessorComponentCollection();
            Pipeline = new MicroProcessorPipelineFactoryBuilder();
        }

        public MicroProcessorComponentCollection Components
        {
            get;
        }              

        public MicroProcessorPipelineFactoryBuilder Pipeline
        {
            get;         
        }

        public IServiceCollection BuildServiceCollection(IServiceCollection services = null)
        {            
            foreach (var builder in Builders())
            {
                services = builder.BuildServiceCollection(services);
            }
            return services.AddSingleton<IMicroProcessor, TProcessor>();
        }

        private IEnumerable<IServiceCollectionBuilder> Builders()
        {
            yield return Components;            
            yield return Pipeline;
        }
    }
}
