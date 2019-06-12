using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroProcessorBuilder<TProcessor> : IMicroProcessorBuilder, IServiceCollectionBuilder
        where TProcessor : class, IMicroProcessor
    {
        private readonly MicroProcessorOptions _options;
        private readonly MicroProcessorComponentCollection _components;

        public MicroProcessorBuilder()
        {   
            _options = new MicroProcessorOptions();
            _components = new MicroProcessorComponentCollection();            
        }

        public UnitOfWorkMode UnitOfWorkMode
        {
            get => _options.UnitOfWorkMode;
            set => _options.UnitOfWorkMode = value;
        }

        public MicroProcessorComponentCollection Components =>
            _components;

        public IServiceCollection BuildServiceCollection(IServiceCollection services = null) =>
            BuildServiceCollection(services ?? new ServiceCollection(), _options.Copy());

        private IServiceCollection BuildServiceCollection(IServiceCollection services, MicroProcessorOptions options)
        {
            foreach (var builder in Builders())
            {
                services = builder.BuildServiceCollection(services);
            }
            return services
                .AddTransient(provider => options)
                .AddTransient<IMicroProcessor, TProcessor>(provider => provider.GetRequiredService<TProcessor>())
                .AddTransient<TProcessor>();
        }

        private IEnumerable<IServiceCollectionBuilder> Builders()
        {
            yield return Components;                        
        }        
    }
}
