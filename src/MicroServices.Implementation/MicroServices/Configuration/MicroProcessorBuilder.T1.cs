using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    internal sealed class MicroProcessorBuilder<TProcessor> : IMicroProcessorBuilder
        where TProcessor : MicroProcessor
    {
        private readonly MicroProcessorOptions _options;
        private readonly MessageHandlerCollection _messageHandlers;
        private readonly QueryCollection _queries;
        private readonly RepositoryCollection _repositories;
        private readonly MicroServiceBusControllerCollection _microServiceBusControllers;
        private readonly MessageIdFactoryCollection _messageIdFactory;
        private readonly List<MicroProcessorComponentCollection> _components;

        public MicroProcessorBuilder()
        {   
            _options = new MicroProcessorOptions();
            _messageHandlers = new MessageHandlerCollection();
            _queries = new QueryCollection();
            _repositories = new RepositoryCollection();
            _microServiceBusControllers = new MicroServiceBusControllerCollection();
            _messageIdFactory = new MessageIdFactoryCollection();
            _components = new List<MicroProcessorComponentCollection>();
        }

        public UnitOfWorkMode UnitOfWorkMode
        {
            get => _options.UnitOfWorkMode;
            set => _options.UnitOfWorkMode = value;
        }

        public MicroProcessorEndpointOptions Endpoints =>
            _options.Endpoints;

        public MessageHandlerCollection MessageHandlers =>
            _messageHandlers;

        public QueryCollection Queries =>
            _queries;

        public RepositoryCollection Repositories =>
            _repositories;

        public MicroServiceBusControllerCollection MicroServiceBusControllers =>
            _microServiceBusControllers;

        public MessageIdFactoryCollection MessageIdFactories =>
            _messageIdFactory;

        public void Add(MicroProcessorComponentCollection components) =>
            _components.Add(components ?? throw new ArgumentNullException(nameof(components)));

        public IServiceCollection BuildServiceCollection(IServiceCollection services = null) =>
            BuildServiceCollection(services ?? new ServiceCollection(), _options.Copy());

        private IServiceCollection BuildServiceCollection(IServiceCollection services, MicroProcessorOptions options)
        {
            if (MicroProcessorType.IsMicroProcessorType(typeof(TProcessor), out var processor))
            {
                return AddComponentsTo(services)
                    .AddTransient(provider => options)
                    .AddComponent(processor);
            }
            throw NewInvalidProcessorTypeException(typeof(TProcessor));
        }        

        private IServiceCollection AddComponentsTo(IServiceCollection services)
        {
            // First, all basic components added to the collections are registered. Before registration,
            // though, all components are merged, so that each type is only registered once.
            foreach (var component in MergeComponents(ComponentsCollections()))
            {
                services = services.AddComponent(component);
            }

            // Second, we let each collection add its own special types, since some collections need to build
            // and register their own components on the fly.
            foreach (var collection in ComponentsCollections())
            {
                services = collection.AddSpecificComponentsTo(services);
            }
            return services;
        }

        private IEnumerable<MicroProcessorComponentCollection> ComponentsCollections()
        {
            yield return _messageHandlers;
            yield return _queries;
            yield return _repositories;
            yield return _microServiceBusControllers;
            yield return _messageIdFactory;

            foreach (var collection in _components)
            {
                yield return collection;
            }
        }

        private static IEnumerable<MicroProcessorComponent> MergeComponents(IEnumerable<MicroProcessorComponentCollection> componentsToMerge)
        {
            var mergedComponents = new Dictionary<Type, MicroProcessorComponent>();

            foreach (var componentToAdd in componentsToMerge.SelectMany(components => components))
            {
                if (mergedComponents.TryGetValue(componentToAdd.Type, out var component))
                {
                    mergedComponents[componentToAdd.Type] = component.MergeWith(componentToAdd);
                }
                else
                {
                    mergedComponents.Add(componentToAdd.Type, componentToAdd);
                }
            }
            return mergedComponents.Values;
        }

        private static Exception NewInvalidProcessorTypeException(Type processorType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorBuilder_InvalidProcessorType;
            var message = string.Format(messageFormat, processorType.FriendlyName());
            return new InvalidOperationException(message);
        }
    }
}
