using System;
using System.Collections.Generic;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroProcessorBuilder<TProcessor> : IMicroProcessorBuilder, IServiceCollectionBuilder
        where TProcessor : MicroProcessor
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

        public MicroProcessorEndpointOptions Endpoints =>
            _options.Endpoints;

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
            if (MicroProcessorType.IsMicroProcessorType(typeof(TProcessor), out var processor))
            {
                return services
                    .AddTransient(provider => options)
                    .AddComponent(processor);
            }
            throw NewInvalidProcessorTypeException(typeof(TProcessor));
        }        

        private IEnumerable<IServiceCollectionBuilder> Builders()
        {
            yield return Components;                        
        }

        private static Exception NewInvalidProcessorTypeException(Type processorType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorBuilder_InvalidProcessorType;
            var message = string.Format(messageFormat, processorType.FriendlyName());
            return new InvalidOperationException(message);
        }
    }
}
